using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using Imgeneus.World.Serialization;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

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

        public static readonly ushort TEST_MAP_ID = 9999;

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

                foreach (var mob in Mobs.Values.Where(m => m.Target == character))
                {
                    mob.ClearTarget();
                };
            }

            return success;
        }

        /// <summary>
        /// Subscribes to character events.
        /// </summary>
        private void AddListeners(Character character)
        {
            // Map with id is test map.
            if (Id == TEST_MAP_ID)
                return;
            character.OnPositionChanged += Character_OnPositionChanged;
            character.OnMotion += Character_OnMotion;
            character.OnEquipmentChanged += Character_OnEquipmentChanged;
            character.OnPartyChanged += Character_OnPartyChanged;
            character.OnAttackOrMoveChanged += Character_OnAttackOrMoveChanged;
            character.OnUsedSkill += Character_OnUsedSkill;
            character.OnAttack += Character_OnAttack;
            character.OnDead += Character_OnDead;
            character.OnSkillCastStarted += Character_OnSkillCastStarted;
            character.OnUsedItem += Character_OnUsedItem;
            character.OnMaxHPChanged += Character_OnMaxHPChanged;
            character.OnRecover += Character_OnRecover;
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
            character.OnAttack -= Character_OnAttack;
            character.OnDead -= Character_OnDead;
            character.OnSkillCastStarted -= Character_OnSkillCastStarted;
            character.OnUsedItem -= Character_OnUsedItem;
            character.OnMaxHPChanged -= Character_OnMaxHPChanged;
            character.OnRecover -= Character_OnRecover;
            character.OnSkillKeep -= Character_OnSkillKeep;
            character.OnShapeChange -= Character_OnShapeChange;
            character.OnUsedRangeSkill -= Character_OnUsedRangeSkill;
            character.OnRebirthed -= Character_OnRebirthed;
        }

        /// <summary>
        /// Teleports player to selected position on map.
        /// </summary>
        /// <param name="playerId">Id of player</param>
        /// <param name="X">new X position</param>
        /// <param name="Y">new Y position</param>
        public void TeleportPlayer(int playerId, float X, float Y)
        {
            if (!Players.ContainsKey(playerId))
            {
                _logger.LogError("Trying to get player, that is not presented on the map.");
            }

            var player = Players[playerId];
            player.UpdatePosition(X, Y, player.PosZ, player.Angle, true, true);
            foreach (var p in Players)
                _packetHelper.SendCharacterTeleport(p.Value.Client, player);
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
        private void Character_OnUsedSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterUsedSkill(player.Value.Client, (Character)sender, target, skill, attackResult);
        }

        /// <summary>
        /// Notifies other players, that player used auto attack.
        /// </summary>
        private void Character_OnAttack(IKiller sender, IKillable target, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendCharacterUsualAttack(player.Value.Client, sender, target, attackResult);
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

        private void Character_OnRecover(IKillable sender, int hp, int mp, int sp)
        {
            foreach (var player in Players)
                _packetHelper.SendRecoverCharacter(player.Value.Client, sender, hp, mp, sp);
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

        private void Character_OnUsedRangeSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendUsedRangeSkill(player.Value.Client, (Character)sender, target, skill, attackResult);
        }

        private void Character_OnRebirthed(IKillable sender)
        {
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterRebirth(player.Value.Client, sender);
                _packetHelper.SendDeadRebirth(player.Value.Client, (Character)sender);
                _packetHelper.SendRecoverCharacter(player.Value.Client, sender, sender.CurrentHP, sender.CurrentMP, sender.CurrentSP);
            }
        }

        #endregion

        #region Mobs

        private int _currentGlobalMobId;
        private readonly object _currentGlobalMobIdMutex = new object();

        /// <summary>
        /// Each entity in game has its' own id.
        /// Call this method, when you need to get new id.
        /// </summary>
        private int GenerateId()
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
            var id = GenerateId();
            var success = Mobs.TryAdd(id, mob);
            if (success)
            {
                mob.Id = id;
                AddListeners(mob);

                _logger.LogDebug($"Mob {mob.MobId} with global id {mob.Id} entered map {Id}");

                foreach (var player in Players)
                    _packetHelper.SendMobEntered(player.Value.Client, mob, true);
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
            mob.OnAttack += Mob_OnAttack;
            mob.OnUsedSkill += Mob_OnUsedSkill;
            mob.OnFullRecover += Mob_OnRecover;
        }

        /// <summary>
        /// Removes listeners from mob.
        /// </summary>
        /// <param name="mob">mob, that we listen</param>
        private void RemoveListeners(Mob mob)
        {
            mob.OnDead -= Mob_OnDead;
            mob.OnMove -= Mob_OnMove;
            mob.OnAttack -= Mob_OnAttack;
            mob.OnUsedSkill -= Mob_OnUsedSkill;
            mob.OnFullRecover -= Mob_OnRecover;
        }

        private void Mob_OnDead(IKillable sender, IKiller killer)
        {
            var mob = (Mob)sender;
            RemoveListeners(mob);
            if (mob.ShouldRebirth)
                mob.TimeToRebirth += RebirthMob;

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
                _packetHelper.SendMobMove(player.Value.Client, sender);
        }

        private void Mob_OnAttack(IKiller sender, IKillable target, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendMobAttack(player.Value.Client, (Mob)sender, target.Id, attackResult);
        }

        private void Mob_OnUsedSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in Players)
                _packetHelper.SendMobUsedSkill(player.Value.Client, (Mob)sender, target.Id, skill, attackResult);
        }

        private void Mob_OnRecover(IKillable sender)
        {
            foreach (var player in Players)
                _packetHelper.SendMobRecover(player.Value.Client, sender);
        }

        /// <summary>
        /// Called, when mob is rebirthed.
        /// </summary>
        /// <param name="sender">rebirthed mob</param>
        public void RebirthMob(Mob sender)
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

        #region Items

        /// <summary>
        /// Dropped items.
        /// </summary>
        private readonly ConcurrentDictionary<int, (Item Item, float X, float Y, float Z)> Items = new ConcurrentDictionary<int, (Item, float, float, float)>();

        /// <summary>
        /// Adds item on map.
        /// </summary>
        /// <param name="item">new added item</param>
        public void AddItem(Item item, float x, float y, float z)
        {
            item.Id = GenerateId();
            if (Items.TryAdd(item.Id, (item, x, y, z)))
            {
                foreach (var player in Players)
                {
                    _packetHelper.SendAddItem(player.Value.Client, item, x, y, z);
                }
            }
        }

        /// <summary>
        /// Tries to get item from map.
        /// </summary>
        /// <returns>if item is null, means that item doen't belong to player yet</returns>
        public Item GetItem(int itemId, Character requester)
        {
            if (Items.TryGetValue(itemId, out var tuple))
            {
                if (tuple.Item.Owner == null || tuple.Item.Owner == requester)
                {
                    return tuple.Item;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes item from map.
        /// </summary>
        public void RemoveItem(int itemId)
        {
            if (Items.TryRemove(itemId, out var tuple))
            {
                foreach (var player in Players)
                {
                    _packetHelper.SendRemoveItem(player.Value.Client, tuple.Item);
                }
            }
        }
        #endregion

        #region NPC

        /// <summary>
        /// Thread-safe dictionary of npcs. Key is npc id, value is npc.
        /// </summary>
        private readonly ConcurrentDictionary<int, Npc> NPCs = new ConcurrentDictionary<int, Npc>();

        /// <summary>
        /// Adds npc to the map.
        /// </summary>
        /// <param name="npc">new npc</param>
        public void AddNPC(Npc npc)
        {
            npc.Id = GenerateId();
            if (NPCs.TryAdd(npc.Id, npc))
            {
                foreach (var player in Players)
                    _packetHelper.SendNpcEnter(player.Value.Client, npc);
            }
        }

        /// <summary>
        /// Removes NPC from the map.
        /// </summary>
        public void RemoveNPC(byte type, ushort typeId, byte count)
        {
            var npcs = NPCs.Values.Where(n => n.Type == type && n.TypeId == typeId).Take(count);
            foreach (var npc in npcs)
            {
                if (NPCs.TryRemove(npc.Id, out var removedNpc))
                {
                    foreach (var player in Players)
                        _packetHelper.SendNpcLeave(player.Value.Client, npc);
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets enemies near target.
        /// </summary>
        public IEnumerable<IKillable> GetEnemies(Character sender, IKillable target, byte range)
        {
            IEnumerable<IKillable> mobs = Mobs.Values.Where(m => !m.IsDead && MathExtensions.Distance(target.PosX, m.PosX, target.PosZ, m.PosZ) <= range);
            IEnumerable<IKillable> chars = Players.Values.Where(p => !p.IsDead && p.Country != sender.Country && MathExtensions.Distance(target.PosX, p.PosX, target.PosZ, p.PosZ) <= range);

            return mobs.Concat(chars);
        }

        /// <summary>
        /// Gets player near point.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <param name="range">minimum range to target, if set to 0 is not calculated</param>
        /// <param name="fraction">light, dark or both</param>
        /// <param name="includeDead">include dead players or not</param>
        public IEnumerable<IKillable> GetPlayers(float x, float z, byte range, Fraction fraction = Fraction.NotSelected, bool includeDead = false)
        {
            return Players.Values.Where(
                p => (includeDead || !p.IsDead) && // filter by death
                     (p.Country == fraction || fraction == Fraction.NotSelected) && // filter by fraction
                     (range == 0 || MathExtensions.Distance(x, p.PosX, z, p.PosZ) <= range)); // filter by range
        }

        #endregion
    }
}
