using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Extensions;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone.MapConfig;
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

        private readonly MapConfiguration _config;
        private readonly ILogger<Map> _logger;
        private readonly IDatabasePreloader _databasePreloader;
        private readonly MapPacketsHelper _packetHelper;

        /// <summary>
        /// Map id.
        /// </summary>
        public ushort Id { get => _config.Id; }

        public static readonly ushort TEST_MAP_ID = 9999;

        public Map(MapConfiguration config, ILogger<Map> logger, IDatabasePreloader databasePreloader)
        {
            _config = config;
            _logger = logger;
            _databasePreloader = databasePreloader;
            _packetHelper = new MapPacketsHelper();

            Init();
        }

        /// <summary>
        /// Inits mobs, npcs, portals etc. based on map configuration.
        /// </summary>
        private void Init()
        {
            CalculateCells(_config.Size, _config.CellSize);

            // TODO: init map.

            // Create npcs.
            foreach (var conf in _config.NPCs)
            {
                if (_databasePreloader.NPCs.TryGetValue((conf.Type, conf.TypeId), out var dbNpc))
                {
                    var moveCoordinates = conf.Coordinates.Select(c => (c.X, c.Y, c.Z, Convert.ToUInt16(c.Angle))).ToList();
                    var npc = new Npc(DependencyContainer.Instance.Resolve<ILogger<Npc>>(), dbNpc, moveCoordinates, this);
                    var cellIndex = GetCellIndex(npc);
                    AddNPC(cellIndex, npc);
                }
            }
        }

        #endregion

        #region Cells

        /// <summary>
        /// Map size.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Minimum cell size.
        /// </summary>
        public int MinCellSize { get; private set; }

        /// <summary>
        /// Number of cells rows.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// Number of cells columns.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// For better performance updates are sent through map cells.
        /// </summary>
        public List<MapCell> Cells { get; private set; } = new List<MapCell>();

        /// <summary>
        /// For better performance map sends updates not about the whole map,
        /// but based on cells. Each map is responsible for its cells update.
        /// </summary>
        /// <param name="size">map size</param>
        public void CalculateCells(int size, int cellSize)
        {
            Size = size;
            MinCellSize = cellSize;

            var mod = Size / MinCellSize;
            var div = Size % MinCellSize;

            Rows = div == 0 ? mod : mod + 1;
            Columns = Rows;

            for (var i = 0; i < Rows * Columns; i++)
            {
                Cells.Add(new MapCell(i, GetNeighborCellIndexes(i), this));
            }
        }

        /// <summary>
        /// Each map member is assigned cell index as soon as he enters into map or moves.
        /// </summary>
        /// <param name="member">map member</param>
        /// <returns>cell index of this map member</returns>
        public int GetCellIndex(IMapMember member)
        {
            if (member.PosX == 0 || member.PosZ == 0)
                return 0;

            int row = ((int)Math.Round(member.PosX, 0)) / MinCellSize;
            int column = ((int)Math.Round(member.PosZ, 0)) / MinCellSize;

            return row + (column * Rows);
        }

        /// <summary>
        /// Gets indexes of neighbor cells.
        /// </summary>
        /// <param name="cellIndex">main cell index</param>
        /// <returns>list of neighbor cell indexes</returns>
        public IEnumerable<int> GetNeighborCellIndexes(int cellIndex)
        {
            var neighbors = new List<int>();
            var myRow = cellIndex % Rows;
            var myColumn = (cellIndex - myRow) / Columns;

            var left = cellIndex - 1;
            if (left >= 0 && (left / Columns) == myColumn)
                neighbors.Add(left);

            var right = cellIndex + 1;
            if (right < Rows * Columns && (right / Columns) == myColumn)
                neighbors.Add(right);

            var top = cellIndex - Rows;
            if (top >= 0)
                neighbors.Add(top);

            var bottom = cellIndex + Rows;
            if (bottom < Rows * Columns)
                neighbors.Add(bottom);

            var column = 0;

            var topleft = cellIndex - Rows - 1;
            column = (topleft - (topleft % Rows)) / Columns;
            if (topleft >= 0 && topleft < Rows * Columns && column == myColumn - 1)
                neighbors.Add(topleft);

            var topright = cellIndex - Rows + 1;
            column = (topright - (topright % Rows)) / Columns;
            if (topright >= 0 && topright < Rows * Columns && column == myColumn - 1)
                neighbors.Add(topright);

            var bottomleft = cellIndex + Rows - 1;
            column = (bottomleft - (bottomleft % Rows)) / Columns;
            if (bottomleft >= 0 && bottomleft < Rows * Columns && column == myColumn + 1)
                neighbors.Add(bottomleft);

            var bottomright = cellIndex + Rows + 1;
            column = (bottomright - (bottomright % Rows)) / Columns;
            if (bottomright >= 0 && bottomright < Rows * Columns && column == myColumn + 1)
                neighbors.Add(bottomright);

            return neighbors.OrderBy(i => i);
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
        /// <param name="playerId">id of player, that you are trying to get.</param>
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
                character.Map = this;
                Cells[GetCellIndex(character)].AddPlayer(character);
                character.OnPositionChanged += Character_OnPositionChanged;
                _logger.LogDebug($"Player {character.Id} connected to map {Id}, cell index {character.CellId}.");
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
                Cells[GetCellIndex(character)].RemovePlayer(character);
                character.OnPositionChanged -= Character_OnPositionChanged;
                _logger.LogDebug($"Player {character.Id} left map {Id}");
            }

            return success;
        }

        /// <summary>
        /// Teleports player to selected position on map.
        /// </summary>
        /// <param name="playerId">Id of player</param>
        /// <param name="X">new X position</param>
        /// <param name="Z">new Z position</param>
        public void TeleportPlayer(int playerId, float X, float Z)
        {
            if (!Players.ContainsKey(playerId))
            {
                _logger.LogError("Trying to get player, that is not presented on the map.");
            }

            var player = Players[playerId];
            Cells[GetCellIndex(player)].TeleportPlayer(player, X, Z);
        }

        /// <summary>
        /// When player's position changes, we are checking if player's map cell should be changed.
        /// </summary>
        private void Character_OnPositionChanged(Character sender)
        {
            var newCellId = GetCellIndex(sender);
            var oldCellId = sender.CellId;
            if (oldCellId == newCellId) // All is fine, character is in the right cell
                return;

            // Need to calculate new cell...
            _logger.LogDebug($"Character {sender.Id} change map cell from {oldCellId} to {newCellId}.");
            Cells[newCellId].AddPlayer(sender);
            Cells[oldCellId].RemovePlayer(sender);
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
        /// Tries to add mob to map and notifies other players, that mob arrived.
        /// </summary>
        /// <returns>turue if mob was added, otherwise false</returns>
        public void AddMob(Mob mob)
        {
            mob.Id = GenerateId();
            Cells[GetCellIndex(mob)].AddMob(mob);
            _logger.LogDebug($"Mob {mob.MobId} with global id {mob.Id} entered map {Id}");

            mob.OnDead += Mob_OnDead;
        }

        /// <summary>
        /// Tries to get mob from map.
        /// </summary>
        /// <param name="cellId">map cell index</param>
        /// <param name="mobId">id of mob, that you are trying to get.</param>
        /// <returns>either mob or null if mob is not presented</returns>
        public Mob GetMob(int cellId, int mobId)
        {
            return Cells[cellId].GetMob(mobId);
        }

        /// <summary>
        /// Rebirth mob if needed.
        /// </summary>
        /// <param name="sender">mob</param>
        /// <param name="killer">mob's killer</param>
        private void Mob_OnDead(IKillable sender, IKiller killer)
        {
            var mob = (Mob)sender;
            if (mob.ShouldRebirth)
                mob.TimeToRebirth += RebirthMob;
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

        #endregion

        #region Items

        /// <summary>
        /// Adds item on map.
        /// </summary>
        /// <param name="item">new added item</param>
        public void AddItem(MapItem item)
        {
            item.Id = GenerateId();
            Cells[GetCellIndex(item)].AddItem(item);
        }

        /// <summary>
        /// Tries to get item from map.
        /// </summary>
        /// <returns>if item is null, means that item doen't belong to player yet</returns>
        public MapItem GetItem(int itemId, Character requester)
        {
            return Cells[requester.CellId].GetItem(itemId, requester);
        }

        /// <summary>
        /// Removes item from map.
        /// </summary>
        public void RemoveItem(int cellId, int itemId)
        {
            Cells[cellId].RemoveItem(itemId);
        }

        #endregion

        #region NPC

        /// <summary>
        /// Adds npc to the map.
        /// </summary>
        /// <param name="cellIndex">cell index</param>
        /// <param name="npc">new npc</param>
        public void AddNPC(int cellIndex, Npc npc)
        {
            npc.Id = GenerateId();
            Cells[cellIndex].AddNPC(npc);
        }

        /// <summary>
        /// Removes NPC from the map.
        /// </summary>
        public void RemoveNPC(int cellIndex, byte type, ushort typeId, byte count)
        {
            Cells[cellIndex].RemoveNPC(type, typeId, count);
        }

        /// <summary>
        /// Gets npc by its' id.
        /// </summary>
        public Npc GetNPC(int cellIndex, int id)
        {
            return Cells[cellIndex].GetNPC(id);
        }

        #endregion
    }
}
