using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game.Zone
{
    /// <summary>
    /// Zone, where users, mobs, npc are presented.
    /// </summary>
    public class Map
    {
        #region Constructor

        private readonly ILogger<Map> _logger;
        private readonly MapPacketsHelper _packetHelper;

        /// <summary>
        /// Map id.
        /// </summary>
        public ushort Id { get; private set; }

        public Map(ushort id, ILogger<Map> logger)
        {
            Id = id;
            _logger = logger;
            _packetHelper = new MapPacketsHelper();
        }

        #endregion

        #region Players

        /// <summary>
        /// Thread-safe dictionary of connected players. Key is character id, value is character.
        /// </summary>
        private readonly ConcurrentDictionary<int, Character> Players = new ConcurrentDictionary<int, Character>();

        /// <summary>
        /// Loads player into map.
        /// </summary>
        /// <param name="character">player, that we need to load</param>
        /// <returns>returns true if we could load player to map, otherwise false</returns>
        public bool LoadPlayer(Character character)
        {
            var success = Players.TryAdd(character.Id, character);

            if (success)
            {
                _logger.LogDebug($"Player {character.Id} connected to map {Id}");

                foreach (var loadedPlayer in Players)
                {
                    if (loadedPlayer.Key != character.Id)
                    {
                        // Notify players in this map, that new player arrived.
                        _packetHelper.SendCharacterConnectedToMap(loadedPlayer.Value.Client, character);

                        // Notify new player, about already loaded player.
                        _packetHelper.SendCharacterConnectedToMap(character.Client, loadedPlayer.Value);
                    }
                }

                character.OnPositionChanged += Character_OnPositionChanged;
                character.OnMotion += Character_OnMotion;
                character.OnSeekForTarget += Character_OnTargetChanged;
                character.OnEquipmentChanged += Character_OnEquipmentChanged;
                character.OnPartyChanged += Character_OnPartyChanged;
                character.OnAttackOrMoveChanged += Character_OnAttackOrMoveChanged;
                character.OnUsedSkill += Character_OnUsedSkill;

                if (character.IsAdmin)
                {
                    character.Client.OnPacketArrived += Client_OnPacketArrived;
                }
            }

            return success;
        }

        /// <summary>
        /// Unloads player from map.
        /// </summary>
        /// <param name="character">player, that we need to unload</param>
        /// <returns>returns true if we could unload player to map, otherwise false</returns>
        public bool UnloadPlayer(Character character)
        {
            var success = Players.TryRemove(character.Id, out var removedCharacter);

            if (success)
            {
                _logger.LogDebug($"Player {character.Id} left map {Id}");

                // Send other clients notification, that user has left the map.
                foreach (var player in Players)
                {
                    _packetHelper.SendCharacterLeftMap(player.Value.Client, removedCharacter);
                }

                character.OnPositionChanged -= Character_OnPositionChanged;
                character.OnMotion -= Character_OnMotion;
                character.OnSeekForTarget -= Character_OnTargetChanged;
                character.OnEquipmentChanged -= Character_OnEquipmentChanged;
                character.OnPartyChanged -= Character_OnPartyChanged;
                character.OnAttackOrMoveChanged -= Character_OnAttackOrMoveChanged;
                character.OnUsedSkill -= Character_OnUsedSkill;

                if (character.IsAdmin)
                {
                    character.Client.OnPacketArrived -= Client_OnPacketArrived;
                }
            }

            return success;
        }

        /// <summary>
        /// Notifies other players about position change.
        /// </summary>
        private void Character_OnPositionChanged(Character movedPlayer)
        {
            // Send other clients notification, that user is moving.
            foreach (var player in Players)
            {
                if (player.Key != movedPlayer.Id)
                {
                    _packetHelper.SendCharacterMoves(player.Value.Client, movedPlayer);
                }
            }
        }

        /// <summary>
        /// When player sends motion, we should resend this motion to all other players on this map.
        /// </summary>
        private void Character_OnMotion(Character playerWithMotion, Motion motion)
        {
            // Notify all players about new motion.
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterMotion(player.Value.Client, playerWithMotion.Id, motion);
            }
        }

        /// <summary>
        /// Sets target based on what target character wants to get.
        /// </summary>
        /// <param name="sender">character, that seeks for target</param>
        /// <param name="targetId">target Id</param>
        /// <param name="targetType">mob or another player</param>
        private void Character_OnTargetChanged(Character sender, int targetId, TargetEntity targetType)
        {
            if (targetType == TargetEntity.Mob)
            {
                sender.Target = Mobs[targetId];
            }
            else
            {
                sender.Target = Players[targetId];
            }
        }

        /// <summary>
        /// Notifies other players, that this player changed equipment.
        /// </summary>
        /// <param name="sender">player, that changed equipment</param>
        /// <param name="equipmentItem">item, that was worn</param>
        /// <param name="slot">item slot</param>
        private void Character_OnEquipmentChanged(Character sender, Item equipmentItem, byte slot)
        {
            // Notify all players about new item.
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterChangedEquipment(player.Value.Client, sender.Id, equipmentItem, slot);
            }
        }

        /// <summary>
        ///  Notifies other players, that player entered/left party or got/removed leader.
        /// </summary>
        private void Character_OnPartyChanged(Character sender)
        {
            foreach (var player in Players)
            {
                PartyMemberType type = PartyMemberType.NoParty;

                if (sender.IsPartyLead)
                    type = PartyMemberType.Leader;
                else if (sender.HasParty)
                    type = PartyMemberType.Member;

                _packetHelper.SendCharacterPartyChanged(player.Value.Client, sender.Id, type);
            }
        }

        /// <summary>
        /// Notifies other players, that player changed attack/move speed.
        /// </summary>
        private void Character_OnAttackOrMoveChanged(Character sender)
        {
            foreach (var player in Players)
            {
                _packetHelper.SendAttackAndMovementSpeed(player.Value.Client, sender);
            }
        }

        /// <summary>
        /// ?
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="attackResult"></param>
        private void Character_OnUsedSkill(Character sender, Skill skill, AttackResult attackResult)
        {
            // Notify all players about used skill.
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterUsedSkill(player.Value.Client, sender.SkillPacketType, sender.Id, sender.Target.Id, skill, attackResult);
            }
        }

        /// <summary>
        /// Handles special packets, as GM packets mob creation etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case GMCreateMobPacket gMCreateMobPacket:
                    // TODO: find out way to preload all awailable mobs.
                    using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                    {
                        var mob = Mob.FromDbMob(database.Mobs.First(m => m.Id == gMCreateMobPacket.MobId), DependencyContainer.Instance.Resolve<ILogger<Mob>>());

                        var gmPlayer = Players[((WorldClient)sender).CharID];
                        // TODO: mobs should be generated near character, not on his position directly.
                        mob.PosX = gmPlayer.PosX;
                        mob.PosY = gmPlayer.PosY;
                        mob.PosZ = gmPlayer.PosZ;

                        AddMob(mob);
                    }
                    break;

                case CharacterAttackPacket attackPacket:
                    HandleAttackPacket((WorldClient)sender, attackPacket.TargetId);
                    break;

                case AttackStart attackStartPacket:
                    // Not sure, but maybe I should not permit any attack start?
                    // Maybe I need to move it to character?
                    sender.SendPacket(new Packet(PacketType.ATTACK_START));
                    break;

                case UsedSkillAttackPacket usedSkillAttackPacket:
                    HandleUseSkill(PacketType.USE_ATTACK_SKILL, (WorldClient)sender, usedSkillAttackPacket.Number, usedSkillAttackPacket.TargetId);
                    break;
                case UsedNonTargetSkillPacket usedNonTargetSkillPacket:
                    HandleUseSkill(PacketType.USE_NON_TARGET_SKILL, (WorldClient)sender, usedNonTargetSkillPacket.Number, usedNonTargetSkillPacket.TargetId);
                    break;
            }
        }

        /// <summary>
        /// Handles use skill packets.
        /// </summary>
        /// <param name="sender">world client, client, that used skill</param>
        /// <param name="number">skill number</param>
        /// <param name="targetId">target id</param>
        private void HandleUseSkill(PacketType packetType, WorldClient sender, byte number, int targetId)
        {
            Players[sender.CharID].NextSkillNumber = number;
            Players[sender.CharID].SkillPacketType = packetType;
        }

        /// <summary>
        /// Handles usual(auto) attack.
        /// </summary>
        /// <param name="sender">world client, client, that attacks</param>
        /// <param name="targetId">target id</param>
        private void HandleAttackPacket(WorldClient sender, int targetId)
        {
            // I comment our this code for now, because auto attack is not working properly with timer now.
            // I will uncomment it as soon as it's working with timer.
            //var attackResult = Players[sender.CharID].UsualAttack();

            //foreach (var player in Players)
            //{
            //    _packetHelper.SendCharacterUsualAttack(player.Value.Client, sender.CharID, targetId, attackResult);
            //}
        }

        #endregion

        #region Mobs

        private int _currentGlobalMobId;
        private readonly object _currentGlobalMobIdMutex = new object();

        /// <summary>
        /// Each mob in game has its' own id.
        /// Call this method, when you need to get new mob id.
        /// </summary>
        private int GenerateMobId()
        {
            lock (_currentGlobalMobIdMutex)
            {
                _currentGlobalMobId++;
            }
            return _currentGlobalMobId;
        }

        /// <summary>
        /// Thread-safe dictionary of monsters loaded to this map. Where key id mob id.
        /// </summary>
        private readonly ConcurrentDictionary<int, Mob> Mobs = new ConcurrentDictionary<int, Mob>();

        /// <summary>
        /// Tries to add mob to map and notifies other players, that mob arrived.
        /// </summary>
        /// <returns>turue if mob was added, otherwise false</returns>
        public bool AddMob(Mob mob)
        {
            var id = GenerateMobId();
            var success = Mobs.TryAdd(id, mob);
            if (success)
            {
                mob.Id = id;
                _logger.LogDebug($"Mob {mob.MobId} entered game world");

                foreach (var player in Players)
                {
                    _packetHelper.SendMobEntered(player.Value.Client, mob);
                }


                // TODO: I'm investigating all available mob packets now.
                // Remove it, when start working on AI implementation!

                // Emulates mob move within 3 seconds after it's created.
                //mob.OnMove += (sender) =>
                //{
                //    foreach (var player in Players)
                //    {
                //        _packetHelper.SendMobEntered(player.Value.Client, sender);
                //    }
                //};
                //mob.EmulateMovement();

                // Emulates mob attack within 3 seconds after it's created.
                //mob.OnAttack += (mob, playerId) =>
                //{
                //    // Send notification each player, that mob attacked.
                //    foreach (var player in Players)
                //    {
                //        _packetHelper.SendMobAttack(player.Value.Client, mob, playerId);
                //    }
                //};
                //mob.EmulateAttack(Players.First().Key);
            }

            return success;
        }

        /// <summary>
        /// Tries to get mob from map.
        /// </summary>
        /// <param name="mobId">id of mob, that you are trying to get.</param>
        /// <returns>either mob or null if mob is not presented</returns>
        public Mob GetMob(int mobId)
        {
            Mobs.TryGetValue(mobId, out var mob);
            return mob;
        }

        #endregion

    }
}
