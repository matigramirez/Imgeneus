using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// Tries to get player from map.
        /// </summary>
        /// <param name="mobId">id of player, that you are trying to get.</param>
        /// <returns>either player or null if player is not presented</returns>
        public Character GetPlayer(int playerId)
        {
            Players.TryGetValue(playerId, out var player);
            return player;
        }

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
                character.Map = this;
                AddListeners(character);

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
                character.Map = null;
                // Send other clients notification, that user has left the map.
                foreach (var player in Players)
                {
                    _packetHelper.SendCharacterLeftMap(player.Value.Client, removedCharacter);
                }
                RemoveListeners(character);
            }

            return success;
        }

        /// <summary>
        /// Subscribes to character events.
        /// </summary>
        private void AddListeners(Character character)
        {
            character.OnPositionChanged += Character_OnPositionChanged;
            character.OnMotion += Character_OnMotion;
            character.OnEquipmentChanged += Character_OnEquipmentChanged;
            character.OnPartyChanged += Character_OnPartyChanged;
            character.OnAttackOrMoveChanged += Character_OnAttackOrMoveChanged;
            character.OnUsedSkill += Character_OnUsedSkill;
            character.OnAutoAttack += Character_OnAutoAttack;
            character.OnDead += Character_OnDead;
            character.OnSkillCastStarted += Character_OnSkillCastStarted;
            character.OnUsedItem += Character_OnUsedItem;
            character.OnMaxHPChanged += Character_OnMaxHPChanged;
            character.HP_Changed += Character_HP_Changed;
            character.MP_Changed += Character_MP_Changed;
            character.SP_Changed += Character_SP_Changed;
            character.OnSkillKeep += Character_OnSkillKeep;
            character.OnShapeChange += Character_OnShapeChange;
            character.OnUsedRangeSkill += Character_OnUsedRangeSkill;
            character.OnRebirthed += Character_OnRebirthed;
        }

        /// <summary>
        /// Unsubscribes from character events.
        /// </summary>
        private void RemoveListeners(Character character)
        {
            character.OnPositionChanged -= Character_OnPositionChanged;
            character.OnMotion -= Character_OnMotion;
            character.OnEquipmentChanged -= Character_OnEquipmentChanged;
            character.OnPartyChanged -= Character_OnPartyChanged;
            character.OnAttackOrMoveChanged -= Character_OnAttackOrMoveChanged;
            character.OnUsedSkill -= Character_OnUsedSkill;
            character.OnAutoAttack -= Character_OnAutoAttack;
            character.OnDead -= Character_OnDead;
            character.OnSkillCastStarted -= Character_OnSkillCastStarted;
            character.OnUsedItem -= Character_OnUsedItem;
            character.OnMaxHPChanged -= Character_OnMaxHPChanged;
            character.HP_Changed -= Character_HP_Changed;
            character.MP_Changed -= Character_MP_Changed;
            character.SP_Changed -= Character_SP_Changed;
            character.OnSkillKeep -= Character_OnSkillKeep;
            character.OnShapeChange -= Character_OnShapeChange;
            character.OnUsedRangeSkill -= Character_OnUsedRangeSkill;
            character.OnRebirthed -= Character_OnRebirthed;
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
                    _packetHelper.SendCharacterMoves(player.Value.Client, movedPlayer);
            }
        }

        /// <summary>
        /// When player sends motion, we should resend this motion to all other players on this map.
        /// </summary>
        private void Character_OnMotion(Character playerWithMotion, Motion motion)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterMotion(player.Value.Client, playerWithMotion.Id, motion);
        }

        /// <summary>
        /// Notifies other players, that this player changed equipment.
        /// </summary>
        /// <param name="sender">player, that changed equipment</param>
        /// <param name="equipmentItem">item, that was worn</param>
        /// <param name="slot">item slot</param>
        private void Character_OnEquipmentChanged(Character sender, Item equipmentItem, byte slot)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterChangedEquipment(player.Value.Client, sender.Id, equipmentItem, slot);
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
        private void Character_OnAttackOrMoveChanged(IKillable sender)
        {
            foreach (var player in Players)
                _packetHelper.SendAttackAndMovementSpeed(player.Value.Client, sender);
        }

        /// <summary>
        /// Notifies other players, that player used skill.
        /// </summary>
        private void Character_OnUsedSkill(Character sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterUsedSkill(player.Value.Client, sender, target, skill, attackResult);
        }

        /// <summary>
        /// Notifies other players, that player used auto attack.
        /// </summary>
        private void Character_OnAutoAttack(Character sender, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterUsualAttack(player.Value.Client, sender, sender.Target, attackResult);
        }

        /// <summary>
        /// Notifies other players, that player is dead.
        /// </summary>
        private void Character_OnDead(IKillable sender, IKiller killer)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterKilled(player.Value.Client, (Character)sender, killer);
        }

        /// <summary>
        /// Notifies other players, that player starts casting.
        /// </summary>
        private void Character_OnSkillCastStarted(Character sender, IKillable target, Skill skill)
        {
            foreach (var player in Players)
                _packetHelper.SendSkillCastStarted(player.Value.Client, sender, target, skill);
        }

        /// <summary>
        /// Notifies other players, that player used some item.
        /// </summary>
        private void Character_OnUsedItem(Character sender, Item item)
        {
            foreach (var player in Players)
                _packetHelper.SendUsedItem(player.Value.Client, sender, item);
        }

        private void Character_HP_Changed(IKillable sender, HitpointArgs args)
        {
            //foreach (var player in Players)
            //    _packetHelper.SendRecoverCharacter(player.Value.Client, sender);
        }

        private void Character_MP_Changed(IKillable sender, HitpointArgs args)
        {
            foreach (var player in Players)
                _packetHelper.SendRecoverCharacter(player.Value.Client, sender);
        }

        private void Character_SP_Changed(IKillable sender, HitpointArgs args)
        {
            foreach (var player in Players)
                _packetHelper.SendRecoverCharacter(player.Value.Client, sender);
        }

        private void Character_OnMaxHPChanged(IKillable sender, int maxHP)
        {
            foreach (var player in Players)
                _packetHelper.Send_Max_HP(player.Value.Client, sender.Id, maxHP);
        }

        private void Character_OnSkillKeep(IKillable sender, ActiveBuff buff, AttackResult result)
        {
            foreach (var player in Players)
                _packetHelper.SendSkillKeep(player.Value.Client, sender.Id, buff.SkillId, buff.SkillLevel, result);
        }

        private void Character_OnShapeChange(Character sender)
        {
            foreach (var player in Players)
                _packetHelper.SendShapeUpdate(player.Value.Client, sender);
        }

        private void Character_OnUsedRangeSkill(Character sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendUsedRangeSkill(player.Value.Client, sender, target, skill, attackResult);
        }

        private void Character_OnRebirthed(IKillable sender)
        {
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterRebirth(player.Value.Client, sender);
                _packetHelper.SendDeadRebirth(player.Value.Client, (Character)sender);
                _packetHelper.SendRecoverCharacter(player.Value.Client, sender);
            }
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
                AddListeners(mob);

                _logger.LogDebug($"Mob {mob.MobId} with global id {mob.Id} entered map {Id}");

                foreach (var player in Players)
                {
                    _packetHelper.SendMobEntered(player.Value.Client, mob, true);
                }


                // TODO: I'm investigating all available mob packets now.
                // Remove it, when start working on AI implementation!

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
        /// Adds listeners to mob events.
        /// </summary>
        /// <param name="mob">mob, that we listen</param>
        private void AddListeners(Mob mob)
        {
            mob.OnDead += Mob_OnDead;
            mob.OnMove += Mob_OnMove;
        }

        /// <summary>
        /// Removes listeners from mob.
        /// </summary>
        /// <param name="mob">mob, that we listen</param>
        private void RemoveListeners(Mob mob)
        {
            mob.OnDead -= Mob_OnDead;
            mob.OnMove -= Mob_OnMove;
        }

        private void Mob_OnDead(IKillable sender, IKiller killer)
        {
            var mob = (Mob)sender;
            RemoveListeners(mob);
            if (mob.ShouldRebirth)
                mob.TimeToRebirth += Mob_TimeToRebirth;

            mob.Dispose();
            if (!Mobs.TryRemove(mob.Id, out var removedMob))
            {
                _logger.LogError($"Could not remove mob {mob.Id} from map.");
            }

            foreach (var player in Players)
                _packetHelper.SendMobDead(player.Value.Client, sender, killer);
        }

        private void Mob_OnMove(Mob sender)
        {
            foreach (var player in Players)
            {
                _packetHelper.SendMobMove(player.Value.Client, sender);
            }
        }

        /// <summary>
        /// Called, when mob is rebirthed.
        /// </summary>
        /// <param name="sender">rebirthed mob</param>
        private void Mob_TimeToRebirth(Mob sender)
        {
            // Create mob clone, because we can not reuse the same id.
            var mob = sender.Clone();

            // TODO: generate rebirth coordinates based on the spawn area.
            mob.PosX = sender.PosX;
            mob.PosY = sender.PosY;
            mob.PosZ = sender.PosZ;

            AddMob(mob);
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

        #region Helpers

        /// <summary>
        /// Gets enemies near target.
        /// </summary>
        public IEnumerable<IKillable> GetEnemies(Character sender, IKillable target, byte applyRange)
        {
            IEnumerable<IKillable> mobs = Mobs.Values.Where(m => Distance(target.PosX, m.PosX, target.PosZ, m.PosZ) <= applyRange);
            IEnumerable<IKillable> chars = Players.Values.Where(p => p.Country != sender.Country && Distance(target.PosX, p.PosX, target.PosZ, p.PosZ) <= applyRange);

            return mobs.Concat(chars);
        }

        /// <summary>
        /// Calculates distance between 2 points.
        /// </summary>
        /// <param name="x1">point1 x coordinate</param>
        /// <param name="x2">point2 x coordinate</param>
        /// <param name="z1">point1 z coordinate</param>
        /// <param name="z2">point2 z coordinate</param>
        /// <returns></returns>
        private double Distance(float x1, float x2, float z1, float z2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(z2 - z1, 2));
        }

        #endregion
    }
}
