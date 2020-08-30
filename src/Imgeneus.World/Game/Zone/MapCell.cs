using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            foreach (var loadedPlayer in Players)
                if (loadedPlayer.Key != character.Id)
                {
                    // Notify players in this map, that new player arrived.
                    _packetHelper.SendCharacterConnectedToMap(loadedPlayer.Value.Client, character);

                    // Notify new player, about already loaded player.
                    _packetHelper.SendCharacterConnectedToMap(character.Client, loadedPlayer.Value);
                }


            // Update npcs.
            var oldCellNPCs = Map.Cells[character.OldCellId].GetAllNPCs(true);
            var newCellNPCs = GetAllNPCs(true);

            var npcToLeave = oldCellNPCs.Where(npc => !newCellNPCs.Contains(npc));
            var npcToEnter = newCellNPCs.Where(npc => !oldCellNPCs.Contains(npc));

            foreach (var npc in npcToLeave)
                _packetHelper.SendNpcLeave(character.Client, npc);
            foreach (var npc in npcToEnter)
                _packetHelper.SendNpcEnter(character.Client, npc);
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
        public IEnumerable<Character> GetAllPlayers()
        {
            return Players.Values;
        }

        /// <summary>
        /// Removes player from map cell.
        /// </summary>
        public void RemovePlayer(Character character)
        {
            RemoveListeners(character);
            Players.TryRemove(character.Id, out var removedCharacter);

            foreach (var mob in Mobs.Values.Where(m => m.Target == character))
                mob.ClearTarget();

            foreach (var player in Players)
                _packetHelper.SendCharacterLeftMap(player.Value.Client, removedCharacter);
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

        #region Character listeners

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

            foreach (var player in Players)
                _packetHelper.SendMobEntered(player.Value.Client, mob, true);
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

        /// <summary>
        /// Gets all mobs from map cell.
        /// </summary>
        public IEnumerable<Mob> GetAllMobs()
        {
            return Mobs.Values;
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
            mob.Dispose();
            Mobs.TryRemove(mob.Id, out var removedMob);

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
                foreach (var player in Players)
                    _packetHelper.SendNpcEnter(player.Value.Client, npc);
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
                    foreach (var player in Players)
                        _packetHelper.SendNpcLeave(player.Value.Client, npc);
                }
            }
        }

        /// <summary>
        /// Gets NPC by id.
        /// </summary>
        public Npc GetNPC(int id)
        {
            NPCs.TryGetValue(id, out var resultNpc);
            return resultNpc;
        }

        /// <summary>
        /// Gets all npcs of this cell.
        /// </summary>
        /// <returns>collection of npcs</returns>
        public IEnumerable<Npc> GetAllNPCs(bool withNeighbors)
        {
            if (withNeighbors)
                return NPCs.Values.Concat(NeighborCells.SelectMany(index => Map.Cells[index].GetAllNPCs(false)));
            else
                return NPCs.Values;
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
                foreach (var player in Players)
                    _packetHelper.SendAddItem(player.Value.Client, item);
            }
        }

        /// <summary>
        /// Tries to get item from map cell.
        /// </summary>
        /// <returns>if item is null, means that item doen't belong to player yet</returns>
        public MapItem GetItem(int itemId, Character requester)
        {
            if (Items.TryGetValue(itemId, out var mapItem))
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
            if (Items.TryRemove(itemId, out var mapItem))
            {
                foreach (var player in Players)
                    _packetHelper.SendRemoveItem(player.Value.Client, mapItem);
            }
        }

        #endregion
    }
}
