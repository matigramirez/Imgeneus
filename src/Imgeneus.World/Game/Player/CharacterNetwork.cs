using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Monster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Player
{
    /// <summary>
    /// Handles only TCP communication with client.
    /// </summary>
    public partial class Character
    {
        private WorldClient _client;

        /// <summary>
        /// TCP connection with client.
        /// </summary>
        public WorldClient Client
        {
            get => _client;

            set
            {
                if (_client is null)
                {
                    _client = value;
                    SendCharacterInfo();

                    _client.OnPacketArrived += Client_OnPacketArrived;
                }
                else
                {
                    throw new ArgumentException("TCP connection can not be set twice");
                }
            }
        }

        /// <summary>
        /// Removes TCP connection.
        /// </summary>
        public void ClearConnection()
        {
            _client = null;
        }

        /// <summary>
        /// Tries to handle all packets, that client sends.
        /// </summary>
        /// <param name="sender">TCP connection with client</param>
        /// <param name="packet">packet, that clients sends</param>
        private async void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case ChangeEncryptionPacket changeEcryptionPacket:
                    SendSkillBar();
                    break;
                case LearnNewSkillPacket learnNewSkillPacket:
                    HandleLearnNewSkill(learnNewSkillPacket);
                    break;

                case MoveItemInInventoryPacket itemInInventoryPacket:
                    HandleMoveItem(itemInInventoryPacket);
                    break;

                case MoveCharacterPacket moveCharacterPacket:
                    HandleMove(moveCharacterPacket);
                    break;

                case MotionPacket motionPacket:
                    HandleMotion(motionPacket);
                    break;

                case MobInTargetPacket mobInTargetPacket:
                    HandleMobInTarget(mobInTargetPacket);
                    break;

                case PlayerInTargetPacket playerInTargetPacket:
                    HandlePlayerInTarget(playerInTargetPacket);
                    break;

                case GMGetItemPacket gMGetItemPacket:
                    HandleGMGetItemPacket(gMGetItemPacket);
                    break;

                case SkillBarPacket skillBarPacket:
                    await HandleSkillBarPacket(skillBarPacket);
                    break;

                case AttackStart attackStartPacket:
                    // Not sure, but maybe I should not permit any attack start?
                    sender.SendPacket(new Packet(PacketType.ATTACK_START));
                    break;

                case MobAutoAttackPacket attackPacket:
                    HandleAutoAttackOnMob(attackPacket.TargetId);
                    break;

                case CharacterAutoAttackPacket attackPacket:
                    HandleAutoAttackOnPlayer(attackPacket.TargetId);
                    break;

                case MobSkillAttackPacket usedSkillMobAttackPacket:
                    HandleUseSkillOnMob(usedSkillMobAttackPacket.Number, usedSkillMobAttackPacket.TargetId);
                    break;

                case CharacterSkillAttackPacket usedSkillPlayerAttackPacket:
                    HandleUseSkillOnPlayer(usedSkillPlayerAttackPacket.Number, usedSkillPlayerAttackPacket.TargetId);
                    break;

                case TargetCharacterGetBuffs targetCharacterGetBuffsPacket:
                    HandleGetCharacterBuffs(targetCharacterGetBuffsPacket.TargetId);
                    break;

                case TargetMobGetBuffs targetMobGetBuffsPacket:
                    HandleGetMobBuffs(targetMobGetBuffsPacket.TargetId);
                    break;

                case CharacterShapePacket characterShapePacket:
                    HandleCharacterShape(characterShapePacket.CharacterId);
                    break;

                case UseItemPacket useItemPacket:
                    UseItem(useItemPacket.Bag, useItemPacket.Slot);
                    break;

                case GMCreateMobPacket gMCreateMobPacket:
                    if (!IsAdmin)
                        return;
                    // TODO: find out way to preload all awailable mobs.
                    using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                    {
                        var mob = new Mob(DependencyContainer.Instance.Resolve<ILogger<Mob>>(), _databasePreloader, gMCreateMobPacket.MobId);

                        // TODO: mobs should be generated near character, not on his position directly.
                        mob.PosX = PosX;
                        mob.PosY = PosY;
                        mob.PosZ = PosZ;

                        Map.AddMob(mob);
                    }
                    break;
            }
        }

        #region Startup info senders

        /// <summary>
        /// Sends to client character start-up information.
        /// </summary>
        private void SendCharacterInfo()
        {
            SendDetails();
            SendAdditionalStats();
            SendCurrentHitpoints();
            SendInventoryItems();
            SendLearnedSkills();
            SendActiveBuffs();
            SendMoveAndAttackSpeed();
            SendBlessAmount();
        }

        #endregion

        #region Handlers

        private void HandleGMGetItemPacket(GMGetItemPacket gMGetItemPacket)
        {
            if (!IsAdmin)
            {
                return;
            }

            AddItemToInventory(new Item(_databasePreloader, gMGetItemPacket.Type, gMGetItemPacket.TypeId) { Count = gMGetItemPacket.Count });
        }

        private void HandlePlayerInTarget(PlayerInTargetPacket packet)
        {
            Target = Map.GetPlayer(packet.TargetId);
        }

        private void HandleMobInTarget(MobInTargetPacket packet)
        {
            Target = Map.GetMob(packet.TargetId);
        }

        private void HandleMotion(MotionPacket packet)
        {
            if (packet.Motion == Motion.None || packet.Motion == Motion.Sit)
            {
                Motion = packet.Motion;
            }

            _logger.LogDebug($"Character {Id} sends motion {packet.Motion}");
            OnMotion?.Invoke(this, packet.Motion);
        }

        private void HandleMove(MoveCharacterPacket packet)
        {
            UpdatePosition(packet.X, packet.Y, packet.Z, packet.Angle, packet.MovementType == MovementType.Stopped);
        }

        private void HandleMoveItem(MoveItemInInventoryPacket moveItemPacket)
        {
            var items = MoveItem(moveItemPacket.CurrentBag, moveItemPacket.CurrentSlot, moveItemPacket.DestinationBag, moveItemPacket.DestinationSlot);
            _packetsHelper.SendMoveItemInInventory(Client, items.sourceItem, items.destinationItem);
        }

        private void HandleLearnNewSkill(LearnNewSkillPacket learnNewSkillsPacket)
        {
            LearnNewSkill(learnNewSkillsPacket.SkillId, learnNewSkillsPacket.SkillLevel);
        }

        private async Task HandleSkillBarPacket(SkillBarPacket skillBarPacket)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            // Remove old items.
            var items = database.QuickItems.Where(item => item.Character.Id == this.Id);
            database.QuickItems.RemoveRange(items);

            DbQuickSkillBarItem[] newItems = new DbQuickSkillBarItem[skillBarPacket.QuickItems.Length];
            // Add new items.
            for (var i = 0; i < skillBarPacket.QuickItems.Length; i++)
            {
                var quickItem = skillBarPacket.QuickItems[i];
                newItems[i] = new DbQuickSkillBarItem()
                {
                    Bar = quickItem.Bar,
                    Slot = quickItem.Slot,
                    Bag = quickItem.Bag,
                    Number = quickItem.Number
                };
                newItems[i].CharacterId = Id;
            }
            await database.QuickItems.AddRangeAsync(newItems);
            await database.SaveChangesAsync();
        }

        private void HandleAutoAttackOnMob(int targetId)
        {
            var target = Map.GetMob(targetId);
            Attack(255, target);
        }

        private void HandleAutoAttackOnPlayer(int targetId)
        {
            var target = Map.GetPlayer(targetId);
            Attack(255, target);
        }

        private void HandleUseSkillOnMob(byte number, int targetId)
        {
            var target = Map.GetMob(targetId);
            Attack(number, target);
        }

        private void HandleUseSkillOnPlayer(byte number, int targetId)
        {
            IKillable target = Map.GetPlayer(targetId);
            Attack(number, target);
        }

        private void HandleGetCharacterBuffs(int targetId)
        {
            var target = Map.GetPlayer(targetId);
            _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleGetMobBuffs(int targetId)
        {
            var target = Map.GetMob(targetId);
            _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleCharacterShape(int characterId)
        {
            var character = Map.GetPlayer(characterId);
            _packetsHelper.SendCharacterShape(Client, character);
        }

        #endregion

        #region Senders

        private void SendDetails() => _packetsHelper.SendDetails(Client, this);

        private void SendCurrentHitpoints() => _packetsHelper.SendCurrentHitpoints(Client, this);

        private void SendInventoryItems() => _packetsHelper.SendInventoryItems(Client, InventoryItems);

        private void SendLearnedSkills() => _packetsHelper.SendLearnedSkills(Client, this);

        private void SendActiveBuffs() => _packetsHelper.SendActiveBuffs(Client, ActiveBuffs);

        private void SendAddBuff(ActiveBuff buff) => _packetsHelper.SendAddBuff(Client, buff);

        private void SendRemoveBuff(ActiveBuff buff) => _packetsHelper.SendRemoveBuff(Client, buff);

        private void SendSkillBar() => _packetsHelper.SendSkillBar(Client, QuickItems);

        protected override void SendAdditionalStats()
        {
            if (Client != null) _packetsHelper.SendAdditionalStats(Client, this);
        }

        private void SendMaxHP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.HP);

        private void SendMaxSP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.SP);

        private void SendMaxMP() => _packetsHelper.SendMaxHitpoints(Client, this, HitpointType.MP);

        private void SendAttackStart() => _packetsHelper.SendAttackStart(Client);

        private void SendAutoAttackWrongTarget(IKillable target) => _packetsHelper.SendAutoAttackWrongTarget(Client, this, target);

        private void SendAutoAttackCanNotAttack(IKillable target) => _packetsHelper.SendAutoAttackCanNotAttack(Client, this, target);

        private void SendSkillAttackCanNotAttack(IKillable target, Skill skill) => _packetsHelper.SendSkillAttackCanNotAttack(Client, this, skill, target);

        private void SendSkillWrongTarget(IKillable target, Skill skill) => _packetsHelper.SendSkillWrongTarget(Client, this, skill, target);

        private void SendSkillWrongEquipment(IKillable target, Skill skill) => _packetsHelper.SendSkillWrongEquipment(Client, this, target, skill);

        private void SendNotEnoughMPSP(IKillable target, Skill skill) => _packetsHelper.SendNotEnoughMPSP(Client, this, target, skill);

        private void SendUseSMMP(ushort needMP, ushort needSP) => _packetsHelper.SendUseSMMP(Client, needMP, needSP);

        private void SendCooldownNotOver(IKillable target, Skill skill) => _packetsHelper.SendCooldownNotOver(Client, this, target, skill);

        protected override void SendMoveAndAttackSpeed()
        {
            if (Client != null) _packetsHelper.SendMoveAndAttackSpeed(Client, this);
        }

        private void SendRunMode() => _packetsHelper.SendRunMode(Client, this);

        private void SendTargetAddBuff(IKillable target, ActiveBuff buff) => _packetsHelper.SendTargetAddBuff(Client, target, buff);

        private void SendTargetRemoveBuff(IKillable target, ActiveBuff buff) => _packetsHelper.SendTargetRemoveBuff(Client, target, buff);

        private void TargetChanged(IKillable target)
        {
            if (target is Mob)
            {
                _packetsHelper.SetMobInTarget(Client, (Mob)target);
            }
            else
            {
                _packetsHelper.SetPlayerInTarget(Client, (Character)target);
            }
        }

        private void InventoryItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Case, when we are starting up and all invenroty items are added with AddRange call.
                if (e.NewItems.Count != 1)
                {
                    return;
                }

                if (Client != null)
                    _packetsHelper.SendAddItem(Client, (Item)e.NewItems[0]);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (Client != null)
                    _packetsHelper.SendRemoveItem(Client, (Item)e.OldItems[0], true);
            }
        }

        #endregion

        /// <summary>
        /// NOT READY! ONLY FOR TESTING!
        /// </summary>
        private void SendBlessAmount()
        {
            using var packet = new Packet(PacketType.BLESS_AMOUNT);
            packet.Write((byte)Fraction.Light);

            // Set bless amount to anu of these values.
            // 150  + sp, mp during break
            // 300  + xp during break
            // 1200 + link and extrack lapis 
            // 1350 + cast time of dispoable items
            // 1500 + exp loss
            // 2100 + shooting/magic defence power
            // 2250 + physical defence power
            // 2700 + repair cost
            // 8400 + sp, mp during battle
            // 10200 + max sp, mp
            // 12000 + increase in all stats (str, dex rec etc.)
            // 12288 + full bless: increase in critical hit rate, evasion of all attacks (shooting/magic/physical)

            var blessAmount = 12288;
            packet.Write(blessAmount);

            if (blessAmount >= 12288)
            {
                // Bless duration is 10 minutes.
                var fullBlessDuration = TimeSpan.FromMinutes(10);
                var timeElapsed = TimeSpan.FromMinutes(5);

                // Remaning time in milliseconds.
                uint remainingTime = (uint)(fullBlessDuration - timeElapsed).TotalMilliseconds;
                packet.Write(remainingTime);
            }
            else
            {
                // Write no remaing time if it's not full bless.
                packet.Write(0);
            }

            Client.SendPacket(packet);
        }
    }
}
