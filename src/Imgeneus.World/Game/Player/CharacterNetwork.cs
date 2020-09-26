using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.Obelisks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        private IWorldClient _client;

        /// <summary>
        /// TCP connection with client.
        /// </summary>
        public IWorldClient Client
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

                case TargetGetMobStatePacket targetGetMobStatePacket:
                    HandleGetMobState(targetGetMobStatePacket.MobId);
                    break;

                case CharacterShapePacket characterShapePacket:
                    HandleCharacterShape(characterShapePacket.CharacterId);
                    break;

                case UseItemPacket useItemPacket:
                    UseItem(useItemPacket.Bag, useItemPacket.Slot);
                    break;

                case ChatNormalPacket chatNormalPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Normal, chatNormalPacket.Message);
                    break;

                case ChatWhisperPacket chatWisperPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Whisper, chatWisperPacket.Message, chatWisperPacket.TargetName);
                    break;

                case ChatPartyPacket chatPartyPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Party, chatPartyPacket.Message);
                    break;

                case ChatMapPacket chatMapPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.Map, chatMapPacket.Message);
                    break;

                case ChatWorldPacket chatWorldPacket:
                    _chatManager.SendMessage(this, Chat.MessageType.World, chatWorldPacket.Message);
                    break;

                case DuelDefeatPacket duelDefeatPacket:
                    FinishDuel(Duel.DuelCancelReason.AdmitDefeat);
                    break;

                case ChangeAppearancePacket changeAppearancePacket:
                    HandleChangeAppearance(changeAppearancePacket);
                    break;

                case FriendRequestPacket friendRequestPacket:
                    HandleFriendRequest(friendRequestPacket.CharacterName);
                    break;

                case FriendResponsePacket friendResponsePacket:
                    ClearFriend(friendResponsePacket.Accepted);
                    break;

                case FriendDeletePacket friendDeletePacket:
                    DeleteFriend(friendDeletePacket.CharacterId);
                    break;

                case RemoveItemPacket removeItemPacket:
                    var item = InventoryItems.FirstOrDefault(itm => itm.Slot == removeItemPacket.Slot && itm.Bag == removeItemPacket.Bag);
                    if (item is null || item.AccountRestriction == ItemAccountRestrictionType.AccountRestricted || item.AccountRestriction == ItemAccountRestrictionType.CharacterRestricted)
                        return;
                    item.TradeQuantity = removeItemPacket.Count <= item.Count ? removeItemPacket.Count : item.Count;
                    var removedItem = RemoveItemFromInventory(item);
                    _packetsHelper.SendRemoveItem(Client, item, removedItem.Count == item.Count);
                    Map.AddItem(new MapItem(removedItem, this, PosX, PosY, PosZ));
                    break;

                case MapPickUpItemPacket mapPickUpItemPacket:
                    var mapItem = Map.GetItem(mapPickUpItemPacket.ItemId, this);
                    if (mapItem is null)
                    {
                        _packetsHelper.SendItemDoesNotBelong(Client);
                        return;
                    }
                    if (mapItem.Item.Type == Item.MONEY_ITEM_TYPE)
                    {
                        Map.RemoveItem(CellId, mapItem.Id);
                        mapItem.Item.Bag = 1;
                        ChangeGold(Gold + (uint)mapItem.Item.Gem1.TypeId);
                        _packetsHelper.SendAddItem(Client, mapItem.Item);
                    }
                    else
                    {
                        var inventoryItem = AddItemToInventory(mapItem.Item);
                        if (inventoryItem is null)
                        {
                            _packetsHelper.SendFullInventory(Client);
                        }
                        else
                        {
                            Map.RemoveItem(CellId, mapItem.Id);
                            _packetsHelper.SendAddItem(Client, inventoryItem);
                            if (Party != null)
                                Party.MemberGetItem(this, inventoryItem);
                        }
                    }
                    break;

                case NpcBuyItemPacket npcBuyItemPacket:
                    var npc = Map.GetNPC(CellId, npcBuyItemPacket.NpcId);
                    if (npc is null || !npc.ContainsProduct(npcBuyItemPacket.ItemIndex))
                        return;
                    var buyItem = npc.Products[npcBuyItemPacket.ItemIndex];
                    var boughtItem = BuyItem(buyItem, npcBuyItemPacket.Count);
                    if (boughtItem != null)
                        _packetsHelper.SendBoughtItem(Client, boughtItem, Gold);
                    break;

                case NpcSellItemPacket npcSellItemPacket:
                    if (npcSellItemPacket.Bag == 0) // Worn item can not be sold, player should take it off first.
                        return;

                    var itemToSell = InventoryItems.FirstOrDefault(i => i.Bag == npcSellItemPacket.Bag && i.Slot == npcSellItemPacket.Slot);
                    if (itemToSell is null) // Item for sale not found.
                        return;

                    var fullSold = itemToSell.Count <= npcSellItemPacket.Count;

                    var soldItem = SellItem(itemToSell, npcSellItemPacket.Count);
                    if (soldItem != null)
                    {
                        if (fullSold)
                            itemToSell.Count = 0;
                        _packetsHelper.SendSoldItem(Client, itemToSell, Gold);
                    }
                    break;

                case QuestStartPacket questStartPacket:
                    var npcQuestGiver = Map.GetNPC(CellId, questStartPacket.NpcId);
                    if (npcQuestGiver is null || !npcQuestGiver.StartQuestIds.Contains(questStartPacket.QuestId))
                    {
                        _logger.LogWarning($"Trying to start unknown quest {questStartPacket.QuestId} at npc {questStartPacket.NpcId}");
                        return;
                    }

                    var quest = new Quest(_databasePreloader, questStartPacket.QuestId);
                    StartQuest(quest, npcQuestGiver.Id);
                    break;

                case QuestEndPacket questEndPacket:
                    var npcQuestReceiver = Map.GetNPC(CellId, questEndPacket.NpcId);
                    if (npcQuestReceiver is null || !npcQuestReceiver.EndQuestIds.Contains(questEndPacket.QuestId))
                    {
                        _logger.LogWarning($"Trying to finish unknown quest {questEndPacket.QuestId} at npc {questEndPacket.NpcId}");
                        return;
                    }
                    FinishQuest(questEndPacket.QuestId, npcQuestReceiver.Id);
                    break;

                case QuestQuitPacket questQuitPacket:
                    QuitQuest(questQuitPacket.QuestId);
                    break;

                case RebirthPacket rebirthPacket:
                    var spawnCoordinate = Map.GetNearestSpawn(PosX, PosY, PosZ, Country);
                    Rebirth(spawnCoordinate.X, spawnCoordinate.Y, spawnCoordinate.Z);
                    break;

                case PartySearchRegistrationPacket searchPartyPacket:
                    HandleSearchParty();
                    break;

                case UseVehiclePacket useVehiclePacket:
                    if (IsOnVehicle)
                        RemoveVehicle();
                    else
                        CallVehicle();
                    break;

                case GMCreateMobPacket gMCreateMobPacket:
                    if (!IsAdmin)
                        return;
                    // TODO: calculate move area.
                    var moveArea = new MoveArea(PosX > 10 ? PosX - 10 : 1, PosX + 10, PosY > 10 ? PosY - 10 : PosY, PosY + 10, PosZ > 10 ? PosZ - 10 : 1, PosZ + 10);
                    var mob = new Mob(DependencyContainer.Instance.Resolve<ILogger<Mob>>(), _databasePreloader, gMCreateMobPacket.MobId, false, moveArea, Map);

                    Map.AddMob(mob);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMTeleportMapPacket gMMovePacket:
                    if (!IsAdmin)
                        return;

                    if (!_gameWorld.Maps.ContainsKey(gMMovePacket.MapId))
                    {
                        _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_MAP);
                        return;
                    }
                    Teleport(gMMovePacket.MapId, gMMovePacket.X, PosY, gMMovePacket.Z);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    _packetsHelper.SendGmTeleport(Client, this);
                    break;

                case GMCreateNpcPacket gMCreateNpcPacket:
                    if (!IsAdmin)
                        return;

                    if (_databasePreloader.NPCs.TryGetValue((gMCreateNpcPacket.Type, gMCreateNpcPacket.TypeId), out var dbNpc))
                    {
                        var moveCoordinates = new List<(float, float, float, ushort)>()
                        {
                            (PosX, PosY, PosZ, Angle)
                        };
                        Map.AddNPC(CellId, new Npc(DependencyContainer.Instance.Resolve<ILogger<Npc>>(), dbNpc, moveCoordinates, Map));
                        _packetsHelper.SendGmCommandSuccess(Client);
                    }
                    else
                    {
                        _packetsHelper.SendGmCommandError(Client, PacketType.GM_CREATE_NPC);
                    }
                    break;

                case GMRemoveNpcPacket gMRemoveNpcPacket:
                    if (!IsAdmin)
                        return;
                    Map.RemoveNPC(CellId, gMRemoveNpcPacket.Type, gMRemoveNpcPacket.TypeId, gMRemoveNpcPacket.Count);
                    _packetsHelper.SendGmCommandSuccess(Client);
                    break;

                case GMFindPlayerPacket gMFindPlayerPacket:
                    HandleFindPlayerPacket(gMFindPlayerPacket.Name);
                    break;

                case GMSummonPlayerPacket gMSummonPlayerPacket:
                    HandleSummonPlayer(gMSummonPlayerPacket.Name);
                    break;

                case GMTeleportToPlayerPacket gMTeleportToPlayerPacket:
                    HandleTeleportToPlayer(gMTeleportToPlayerPacket.Name);
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
            SendInventoryItems(); // TODO: game.exe crashes, when number of items >= 80. Investigate why?
            SendLearnedSkills();
            SendOpenQuests();
            SendFinishedQuests();
            SendActiveBuffs();
            SendMoveAndAttackSpeed();
            SendFriends();
            SendBlessAmount();
        }

        #endregion

        #region Handlers

        private void HandleGMGetItemPacket(GMGetItemPacket gMGetItemPacket)
        {
            if (!IsAdmin)
                return;

            var item = AddItemToInventory(new Item(_databasePreloader, gMGetItemPacket.Type, gMGetItemPacket.TypeId, gMGetItemPacket.Count));
            if (item != null)
            {
                _packetsHelper.SendAddItem(Client, item);
                _packetsHelper.SendGmCommandSuccess(Client);
            }
            else
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_COMMAND_GET_ITEM);
        }

        private void HandlePlayerInTarget(PlayerInTargetPacket packet)
        {
            Target = Map.GetPlayer(packet.TargetId);
        }

        private void HandleMobInTarget(MobInTargetPacket packet)
        {
            Target = Map.GetMob(CellId, packet.TargetId);
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
            var target = Map.GetMob(CellId, targetId);
            Attack(255, target);
        }

        private void HandleAutoAttackOnPlayer(int targetId)
        {
            var target = Map.GetPlayer(targetId);
            Attack(255, target);
        }

        private void HandleUseSkillOnMob(byte number, int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
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
            if (target != null)
                _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleGetMobBuffs(int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            if (target != null)
                _packetsHelper.SendCurrentBuffs(Client, target);
        }

        private void HandleGetMobState(int targetId)
        {
            var target = Map.GetMob(CellId, targetId);
            if (target != null)
            {
                _packetsHelper.SendMobPosition(Client, target);
                _packetsHelper.SendMobState(Client, target);
            }
            else
                _logger.LogWarning($"Coudn't find mob {targetId} state.");
        }

        private void HandleCharacterShape(int characterId)
        {
            var character = _gameWorld.Players[characterId];
            if (character is null)
            {
                _logger.LogWarning($"Trying to get player {characterId}, that is not presented in game world.");
                return;
            }

            _packetsHelper.SendCharacterShape(Client, character);
        }

        private void HandleChangeAppearance(ChangeAppearancePacket changeAppearancePacket)
        {
            var item = InventoryItems.FirstOrDefault(itm => itm.Slot == changeAppearancePacket.Slot && itm.Bag == changeAppearancePacket.Bag);
            if (item is null || (item.Special != SpecialEffect.AppearanceChange && item.Special != SpecialEffect.SexChange))
                return;

            UseItem(changeAppearancePacket.Bag, changeAppearancePacket.Slot);
            ChangeAppearance(changeAppearancePacket.Face, changeAppearancePacket.Hair, changeAppearancePacket.Size, changeAppearancePacket.Sex);
        }

        private void HandleFriendRequest(string characterName)
        {
            var character = _gameWorld.Players.FirstOrDefault(p => p.Value.Name == characterName).Value;
            if (character is null || character.Country != this.Country)
                return;

            character.RequestFriendship(this);
        }

        private void HandleSearchParty()
        {
            if (Party != null)
                return;

            Map.RegisterSearchForParty(this);
            _packetsHelper.SendRegisteredInPartySearch(Client, true);

            var searchers = Map.PartySearchers.Where(s => s.Country == Country && s != this);
            if (searchers.Any())
                _packetsHelper.SendPartySearchList(Client, searchers.Take(30));
        }

        private async void HandleSummonPlayer(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);

            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_SUMMON_PLAYER);
            else
            {
                player.Teleport(MapId, PosX, PosY, PosZ);

                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendGmSummon(player.Client, player);
                await Task.Delay(100);
                _packetsHelper.SendCharacterTeleport(player.Client, player);
            }
        }

        private void HandleFindPlayerPacket(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);
            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_FIND_PLAYER);
            else
            {
                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendCharacterPosition(Client, player);
            }
        }

        private void HandleTeleportToPlayer(string playerName)
        {
            if (!IsAdmin)
                return;

            var player = _gameWorld.Players.Values.FirstOrDefault(p => p.Name == playerName);
            if (player is null)
                _packetsHelper.SendGmCommandError(Client, PacketType.GM_TELEPORT_TO_PLAYER);
            else
            {
                Teleport(player.MapId, player.PosX, player.PosY, player.PosZ);

                _packetsHelper.SendGmCommandSuccess(Client);
                _packetsHelper.SendCharacterTeleport(Client, this);
                _packetsHelper.SendGmTeleportToPlayer(Client, player);
            }
        }

        #endregion

        #region Senders

        private void SendDetails() => _packetsHelper.SendDetails(Client, this);

        protected override void SendCurrentHitpoints() => _packetsHelper.SendCurrentHitpoints(Client, this);

        private void SendInventoryItems() => _packetsHelper.SendInventoryItems(Client, InventoryItems);

        private void SendLearnedSkills() => _packetsHelper.SendLearnedSkills(Client, this);

        private void SendOpenQuests() => _packetsHelper.SendQuests(Client, Quests.Where(q => !q.IsFinished));

        private void SendFinishedQuests() => _packetsHelper.SendFinishedQuests(Client, Quests.Where(q => q.IsFinished));

        private void SendQuestStarted(Quest quest, int npcId = 0) => _packetsHelper.SendQuestStarted(Client, quest.Id, npcId);

        private void SendQuestFinished(Quest quest, int npcId = 0) => _packetsHelper.SendQuestFinished(Client, quest, npcId);

        private void SendFriendRequest(Character requester) => _packetsHelper.SendFriendRequest(Client, requester);

        private void SendFriendOnline(int friendId, bool isOnline) => _packetsHelper.SendFriendOnline(Client, friendId, isOnline);

        private void SendFriends() => _packetsHelper.SendFriends(Client, Friends.Values);

        private void SendFriendAdd(Character friend) => _packetsHelper.SendFriendAdded(Client, friend);

        private void SendFriendResponse(bool accepted) => _packetsHelper.SendFriendResponse(Client, accepted);

        private void SendFriendDelete(int id) => _packetsHelper.SendFriendDelete(Client, id);

        private void SendQuestCountUpdate(ushort questId, byte index, byte count) => _packetsHelper.SendQuestCountUpdate(Client, questId, index, count);

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

        public void SendAddItemToInventory(Item item) => _packetsHelper.SendAddItem(Client, item);

        public void SendRemoveItemFromInventory(Item item, bool fullRemove) => _packetsHelper.SendRemoveItem(Client, item, fullRemove);

        public void SendWeather() => _packetsHelper.SendWeather(Client, Map);

        public void SendObelisks() => _packetsHelper.SendObelisks(Client, Map.Obelisks.Values);

        public void SendObeliskBroken(Obelisk obelisk) => _packetsHelper.SendObeliskBroken(Client, obelisk);

        public void SendCharacterTeleport() => _packetsHelper.SendCharacterTeleport(Client, this);

        public void SendUseVehicle(bool success, bool status) => _packetsHelper.SendUseVehicle(Client, success, status);

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

        #endregion
    }
}
