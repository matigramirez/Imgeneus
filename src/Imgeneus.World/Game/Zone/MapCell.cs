using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.Zone
{
    public class MapCell
    {
        private readonly MapPacketsHelper _packetHelper = new MapPacketsHelper();

        public MapCell(int index, IEnumerable<int> neighborCells, Map map)
        {
            CellIndex = index;
            NeighborCells = neighborCells;
            Map = map;
        }

        public int CellIndex { get; private set; }

        public IEnumerable<int> NeighborCells { get; private set; }

        private readonly Map Map;

        /// <summary>
        /// Sets cell index to each cell member.
        /// </summary>
        private void AssignCellIndex(IMapMember member)
        {
            member.OldCellId = member.CellId;
            member.CellId = CellIndex;
        }

        #region Players

        /// <summary>
        /// Thread-safe dictionary of connected players. Key is character id, value is character.
        /// </summary>
        private readonly ConcurrentDictionary<int, Character> Players = new ConcurrentDictionary<int, Character>();

        /// <summary>
        /// Adds character to map cell.
        /// </summary>
        public void AddPlayer(Character character)
        {
            Players.TryAdd(character.Id, character);
            AddListeners(character);
            AssignCellIndex(character);

            // Send update players.
            var oldPlayers = character.OldCellId != -1 ? Map.Cells[character.OldCellId].GetAllPlayers(true) : new List<Character>();
            var newPlayers = GetAllPlayers(true);

            var sendPlayerLeave = oldPlayers.Where(p => !newPlayers.Contains(p) && p != character);
            var sendPlayerEnter = newPlayers.Where(p => !oldPlayers.Contains(p));

            foreach (var player in sendPlayerLeave)
            {
                _packetHelper.SendCharacterLeftMap(player.Client, character);
                _packetHelper.SendCharacterLeftMap(character.Client, player);
            }

            foreach (var player in sendPlayerEnter)
                if (player.Id != character.Id)
                {
                    // Notify players in this map, that new player arrived.
                    _packetHelper.SendCharacterConnectedToMap(player.Client, character);

                    // Notify new player, about already loaded player.
                    _packetHelper.SendCharacterConnectedToMap(character.Client, player);
                }

            // Send update npcs.
            var oldCellNPCs = character.OldCellId != -1 ? Map.Cells[character.OldCellId].GetAllNPCs(true) : new List<Npc>();
            var newCellNPCs = GetAllNPCs(true);

            var npcToLeave = oldCellNPCs.Where(npc => !newCellNPCs.Contains(npc));
            var npcToEnter = newCellNPCs.Where(npc => !oldCellNPCs.Contains(npc));

            foreach (var npc in npcToLeave)
                _packetHelper.SendNpcLeave(character.Client, npc);
            foreach (var npc in npcToEnter)
                _packetHelper.SendNpcEnter(character.Client, npc);

            // Send update mobs.
            var oldCellMobs = character.OldCellId != -1 ? Map.Cells[character.OldCellId].GetAllMobs(true) : new List<Mob>();
            var newCellMobs = GetAllMobs(true);

            var mobToLeave = oldCellMobs.Where(m => !newCellMobs.Contains(m));
            var mobToEnter = newCellMobs.Where(m => !oldCellMobs.Contains(m));

            foreach (var mob in mobToLeave)
                _packetHelper.SendMobLeave(character.Client, mob);

            foreach (var mob in mobToEnter)
                _packetHelper.SendMobEnter(character.Client, mob, false);
        }

        /// <summary>
        /// Tries to get player from map cell.
        /// </summary>
        /// <param name="playerId">id of player, that you are trying to get.</param>
        /// <returns>either player or null if player is not presented</returns>
        public Character GetPlayer(int playerId)
        {
            Players.TryGetValue(playerId, out var player);
            return player;
        }

        /// <summary>
        /// Gets all players from map cell.
        /// </summary>
        /// <param name="includeNeighborCells">if set to true includes characters fom neigbour cells</param>
        public IEnumerable<Character> GetAllPlayers(bool includeNeighborCells)
        {
            var myPlayers = Players.Values;
            if (includeNeighborCells)
                return myPlayers.Concat(NeighborCells.Select(index => Map.Cells[index]).SelectMany(cell => cell.GetAllPlayers(false))).Distinct();
            return myPlayers;
        }

        /// <summary>
        /// Gets player near point.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <param name="range">minimum range to target, if set to 0 is not calculated</param>
        /// <param name="fraction">light, dark or both</param>
        /// <param name="includeDead">include dead players or not</param>
        /// <param name="includeNeighborCells">include players from neighbor cells, usually true</param>
        public IEnumerable<IKillable> GetPlayers(float x, float z, byte range, Fraction fraction = Fraction.NotSelected, bool includeDead = false, bool includeNeighborCells = true)
        {
            var myPlayers = Players.Values.Where(
                     p => (includeDead || !p.IsDead) && // filter by death
                     (p.Country == fraction || fraction == Fraction.NotSelected) && // filter by fraction
                     (range == 0 || MathExtensions.Distance(x, p.PosX, z, p.PosZ) <= range)); // filter by range
            if (includeNeighborCells)
                return myPlayers.Concat(NeighborCells.Select(index => Map.Cells[index]).SelectMany(cell => cell.GetPlayers(x, z, range, fraction, includeDead, false))).Distinct();
            return myPlayers;
        }

        /// <summary>
        /// Gets enemies near target.
        /// </summary>
        public IEnumerable<IKillable> GetEnemies(Character sender, IKillable target, byte range)
        {
            IEnumerable<IKillable> mobs = GetAllMobs(true).Where(m => !m.IsDead && MathExtensions.Distance(target.PosX, m.PosX, target.PosZ, m.PosZ) <= range);
            IEnumerable<IKillable> chars = GetAllPlayers(true).Where(p => !p.IsDead && p.Country != sender.Country && MathExtensions.Distance(target.PosX, p.PosX, target.PosZ, p.PosZ) <= range);

            return mobs.Concat(chars);
        }

        /// <summary>
        /// Removes player from map cell.
        /// </summary>
        public void RemovePlayer(Character character, bool notifyPlayers)
        {
            RemoveListeners(character);
            Players.TryRemove(character.Id, out var removedCharacter);

            foreach (var mob in GetAllMobs(true).Where(m => m.Target == character))
                mob.ClearTarget();

            if (notifyPlayers)
                foreach (var player in GetAllPlayers(true))
                    _packetHelper.SendCharacterLeftMap(player.Client, character);
        }

        /// <summary>
        /// Teleports player to new position.
        /// </summary>
        public void TeleportPlayer(Character character, float X, float Z)
        {
            character.UpdatePosition(X, character.PosY, Z, character.Angle, true, true);
            foreach (var p in Players)
                _packetHelper.SendCharacterTeleport(p.Value.Client, character);
        }

        /// <summary>
        /// Subscribes to character events.
        /// </summary>
        private void AddListeners(Character character)
        {
            // Map with id is test map.
            if (character.Map.Id == Map.TEST_MAP_ID)
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
            character.OnAppearanceChanged += Character_OnAppearanceChanged;
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
            character.OnAppearanceChanged -= Character_OnAppearanceChanged;
        }

        #region Character listeners

        /// <summary>
        /// Notifies other players about position change.
        /// </summary>
        private void Character_OnPositionChanged(Character movedPlayer)
        {
            // Send other clients notification, that user is moving.
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterMoves(player.Client, movedPlayer);
        }

        /// <summary>
        /// When player sends motion, we should resend this motion to all other players on this map.
        /// </summary>
        private void Character_OnMotion(Character playerWithMotion, Motion motion)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterMotion(player.Client, playerWithMotion.Id, motion);
        }

        /// <summary>
        /// Notifies other players, that this player changed equipment.
        /// </summary>
        /// <param name="sender">player, that changed equipment</param>
        /// <param name="equipmentItem">item, that was worn</param>
        /// <param name="slot">item slot</param>
        private void Character_OnEquipmentChanged(Character sender, Item equipmentItem, byte slot)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterChangedEquipment(player.Client, sender.Id, equipmentItem, slot);
        }

        /// <summary>
        ///  Notifies other players, that player entered/left party or got/removed leader.
        /// </summary>
        private void Character_OnPartyChanged(Character sender)
        {
            foreach (var player in GetAllPlayers(true))
            {
                PartyMemberType type = PartyMemberType.NoParty;

                if (sender.IsPartyLead)
                    type = PartyMemberType.Leader;
                else if (sender.HasParty)
                    type = PartyMemberType.Member;

                _packetHelper.SendCharacterPartyChanged(player.Client, sender.Id, type);
            }
        }

        /// <summary>
        /// Notifies other players, that player changed attack/move speed.
        /// </summary>
        private void Character_OnAttackOrMoveChanged(IKillable sender)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendAttackAndMovementSpeed(player.Client, sender);
        }

        /// <summary>
        /// Notifies other players, that player used skill.
        /// </summary>
        private void Character_OnUsedSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterUsedSkill(player.Client, (Character)sender, target, skill, attackResult);
        }

        /// <summary>
        /// Notifies other players, that player used auto attack.
        /// </summary>
        private void Character_OnAttack(IKiller sender, IKillable target, AttackResult attackResult)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterUsualAttack(player.Client, sender, target, attackResult);
        }

        /// <summary>
        /// Notifies other players, that player is dead.
        /// </summary>
        private void Character_OnDead(IKillable sender, IKiller killer)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendCharacterKilled(player.Client, (Character)sender, killer);
        }

        /// <summary>
        /// Notifies other players, that player starts casting.
        /// </summary>
        private void Character_OnSkillCastStarted(Character sender, IKillable target, Skill skill)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendSkillCastStarted(player.Client, sender, target, skill);
        }

        /// <summary>
        /// Notifies other players, that player used some item.
        /// </summary>
        private void Character_OnUsedItem(Character sender, Item item)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendUsedItem(player.Client, sender, item);
        }

        private void Character_OnRecover(IKillable sender, int hp, int mp, int sp)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendRecoverCharacter(player.Client, sender, hp, mp, sp);
        }

        private void Character_OnMaxHPChanged(IKillable sender, int maxHP)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.Send_Max_HP(player.Client, sender.Id, maxHP);
        }

        private void Character_OnSkillKeep(IKillable sender, ActiveBuff buff, AttackResult result)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendSkillKeep(player.Client, sender.Id, buff.SkillId, buff.SkillLevel, result);
        }

        private void Character_OnShapeChange(Character sender)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendShapeUpdate(player.Client, sender);
        }

        private void Character_OnUsedRangeSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendUsedRangeSkill(player.Client, (Character)sender, target, skill, attackResult);
        }

        private void Character_OnRebirthed(IKillable sender)
        {
            foreach (var player in GetAllPlayers(true))
            {
                _packetHelper.SendCharacterRebirth(player.Client, sender);
                _packetHelper.SendDeadRebirth(player.Client, (Character)sender);
                _packetHelper.SendRecoverCharacter(player.Client, sender, sender.CurrentHP, sender.CurrentMP, sender.CurrentSP);
            }
        }

        private void Character_OnAppearanceChanged(Character sender)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendAppearanceChanged(player.Client, sender);
        }

        #endregion

        #endregion

        #region Mobs

        /// <summary>
        /// Thread-safe dictionary of monsters loaded to this map. Where key id mob id.
        /// </summary>
        private readonly ConcurrentDictionary<int, Mob> Mobs = new ConcurrentDictionary<int, Mob>();

        /// <summary>
        /// Adds mob to cell.
        /// </summary>
        public void AddMob(Mob mob)
        {
            Mobs.TryAdd(mob.Id, mob);
            AssignCellIndex(mob);
            AddListeners(mob);

            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobEnter(player.Client, mob, true);
        }

        /// <summary>
        /// Removes mob from cell.
        /// </summary>
        public void RemoveMob(Mob mob)
        {
            Mobs.TryRemove(mob.Id, out var removedMob);
            RemoveListeners(removedMob);

            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobLeave(player.Client, mob);
        }

        /// <summary>
        /// Tries to get mob from map.
        /// </summary>
        /// <param name="mobId">id of mob, that you are trying to get.</param>
        /// <param name="includeNeighborCells">search also in neighbor cells</param>
        /// <returns>either mob or null if mob is not presented</returns>
        public Mob GetMob(int mobId, bool includeNeighborCells)
        {
            Mob mob;
            Mobs.TryGetValue(mobId, out mob);

            if (mob is null && includeNeighborCells) // Maybe mob in neighbor cell?
                foreach (var cellId in NeighborCells)
                {
                    mob = Map.Cells[cellId].GetMob(mobId, false);
                    if (mob != null)
                        break;
                }

            return mob;
        }

        /// <summary>
        /// Gets all mobs from map cell.
        /// </summary>
        /// /// <param name="includeNeighborCells">if set to true includes mobs fom neigbour cells</param>
        public IEnumerable<Mob> GetAllMobs(bool includeNeighborCells)
        {
            var myMobs = Mobs.Values;
            if (includeNeighborCells)
                return myMobs.Concat(NeighborCells.Select(index => Map.Cells[index]).SelectMany(cell => cell.GetAllMobs(false))).Distinct();
            return myMobs;
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
            Mobs.TryRemove(mob.Id, out var removedMob);

            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobDead(player.Client, sender, killer);
        }

        private void Mob_OnMove(Mob sender)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobMove(player.Client, sender);
        }

        private void Mob_OnAttack(IKiller sender, IKillable target, AttackResult attackResult)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobAttack(player.Client, (Mob)sender, target.Id, attackResult);
        }

        private void Mob_OnUsedSkill(IKiller sender, IKillable target, Skill skill, AttackResult attackResult)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobUsedSkill(player.Client, (Mob)sender, target.Id, skill, attackResult);
        }

        private void Mob_OnRecover(IKillable sender)
        {
            foreach (var player in GetAllPlayers(true))
                _packetHelper.SendMobRecover(player.Client, sender);
        }

        #endregion

        #region NPCs

        /// <summary>
        /// Thread-safe dictionary of npcs. Key is npc id, value is npc.
        /// </summary>
        private readonly ConcurrentDictionary<int, Npc> NPCs = new ConcurrentDictionary<int, Npc>();

        /// <summary>
        /// Adds npc to cell.
        /// </summary>
        /// <param name="npc">npc to add</param>
        public void AddNPC(Npc npc)
        {
            if (NPCs.TryAdd(npc.Id, npc))
            {
                AssignCellIndex(npc);
                foreach (var player in GetAllPlayers(true))
                    _packetHelper.SendNpcEnter(player.Client, npc);
            }
        }

        /// <summary>
        /// Removes npc from cell.
        /// </summary>
        public void RemoveNPC(byte type, ushort typeId, byte count)
        {
            var npcs = NPCs.Values.Where(n => n.Type == type && n.TypeId == typeId).Take(count);
            foreach (var npc in npcs)
            {
                if (NPCs.TryRemove(npc.Id, out var removedNpc))
                {
                    foreach (var player in GetAllPlayers(true))
                        _packetHelper.SendNpcLeave(player.Client, npc);
                }
            }
        }

        /// <summary>
        /// Gets NPC by id.
        /// </summary>
        /// <param name="includeNeighborCells">search also in neighbor cells</param>
        public Npc GetNPC(int id, bool includeNeighborCells)
        {
            Npc npc;
            NPCs.TryGetValue(id, out npc);

            if (npc is null && includeNeighborCells) // Maybe npc in neigbor cell?
                foreach (var cellId in NeighborCells)
                {
                    npc = Map.Cells[cellId].GetNPC(id, false);
                    if (npc != null)
                        break;
                }

            return npc;
        }

        /// <summary>
        /// Gets all npcs of this cell.
        /// </summary>
        /// <returns>collection of npcs</returns>
        public IEnumerable<Npc> GetAllNPCs(bool includeNeighbors)
        {
            var myNPCs = NPCs.Values;
            if (includeNeighbors)
                return myNPCs.Concat(NeighborCells.SelectMany(index => Map.Cells[index].GetAllNPCs(false))).Distinct();
            return myNPCs;
        }

        #endregion

        #region Items

        /// <summary>
        /// Dropped items.
        /// </summary>
        private readonly ConcurrentDictionary<int, MapItem> Items = new ConcurrentDictionary<int, MapItem>();

        /// <summary>
        /// Adds item on map cell.
        /// </summary>
        /// <param name="item">new added item</param>
        public void AddItem(MapItem item)
        {
            if (Items.TryAdd(item.Id, item))
            {
                AssignCellIndex(item);
                foreach (var player in GetAllPlayers(true))
                    _packetHelper.SendAddItem(player.Client, item);
            }
        }

        /// <summary>
        /// Tries to get item from map cell.
        /// </summary>
        /// <returns>if item is null, means that item doen't belong to player yet</returns>
        public MapItem GetItem(int itemId, Character requester, bool includeNeighborCells)
        {
            MapItem mapItem;
            if (Items.TryGetValue(itemId, out mapItem))
            {
                if (mapItem.Owner == null || mapItem.Owner == requester)
                {
                    return mapItem;
                }
                else
                {
                    return null;
                }
            }
            else // Maybe item is in neighbor cell?
            {
                if (includeNeighborCells)
                    foreach (var cellId in NeighborCells)
                    {
                        mapItem = Map.Cells[cellId].GetItem(itemId, requester, false);
                        if (mapItem != null)
                            break;
                    }

                return mapItem;
            }
        }

        /// <summary>
        /// Removes item from map.
        /// </summary>
        public void RemoveItem(int itemId)
        {
            if (Items.TryRemove(itemId, out var mapItem))
            {
                foreach (var player in GetAllPlayers(true))
                    _packetHelper.SendRemoveItem(player.Client, mapItem);
            }
        }

        #endregion
    }
}
