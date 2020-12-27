using Imgeneus.Database;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Blessing;
using Imgeneus.World.Game.Duel;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Trade;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Portals;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public class GameWorld : IGameWorld
    {
        private readonly ILogger<GameWorld> _logger;
        private readonly IMapsLoader _mapsLoader;
        private readonly IMapFactory _mapFactory;
        private readonly ICharacterFactory _characterFactory;
        private MapDefinitions _mapDefinitions;

        public GameWorld(ILogger<GameWorld> logger, IMapsLoader mapsLoader, IMapFactory mapFactory, ICharacterFactory characterFactory)
        {
            _logger = logger;
            _mapsLoader = mapsLoader;
            _mapFactory = mapFactory;
            _characterFactory = characterFactory;

            InitMaps();
        }

        #region Maps 

        /// <summary>
        /// Thread-safe dictionary of maps. Where key is map id.
        /// </summary>
        public ConcurrentDictionary<ushort, IMap> Maps { get; private set; } = new ConcurrentDictionary<ushort, IMap>();

        /// <summary>
        /// Initializes maps with startup values like mobs, npc, areas, obelisks etc.
        /// </summary>
        private void InitMaps()
        {
            _mapDefinitions = _mapsLoader.LoadMapDefinitions();
            foreach (var mapDefinition in _mapDefinitions.Maps)
            {
                var config = _mapsLoader.LoadMapConfiguration(mapDefinition.Id);

                if (mapDefinition.CreateType == CreateType.Default)
                {
                    config.Obelisks = _mapsLoader.GetObelisks(mapDefinition.Id);

                    var map = _mapFactory.CreateMap(mapDefinition.Id, mapDefinition, config);
                    if (Maps.TryAdd(mapDefinition.Id, map))
                        _logger.LogInformation($"Map {map.Id} was successfully loaded.");
                }
            }
        }

        /// <inheritdoc />
        public bool CanTeleport(Character player, byte portalIndex, out PortalTeleportNotAllowedReason reason)
        {
            reason = PortalTeleportNotAllowedReason.Unknown;

            var map = player.Map;
            if (map.Portals.Count <= portalIndex)
            {
                _logger.LogWarning($"Unknown portal {portalIndex} for map {map.Id}. Send from character {player.Id}.");
                return false;
            }

            var portal = map.Portals[portalIndex];
            if (!portal.IsInPortalZone(player.PosX, player.PosY, player.PosZ))
            {
                _logger.LogWarning($"Character position is not in portal, map {map.Id}. Portal index {portalIndex}. Send from character {player.Id}.");
                return false;
            }

            if (!portal.IsSameFaction(player.Country))
            {
                return false;
            }

            if (!portal.IsRightLevel(player.Level))
            {
                return false;
            }

            if (Maps.ContainsKey(portal.MapId))
            {
                return true;
            }
            else // Not "usual" map.
            {
                var destinationMapId = portal.MapId;
                var destinationMapDef = _mapDefinitions.Maps.FirstOrDefault(d => d.Id == destinationMapId);

                if (destinationMapDef is null)
                {
                    _logger.LogWarning($"Map {destinationMapId} is not found in map definitions.");
                    return false;
                }

                if (destinationMapDef.CreateType == CreateType.Party)
                {
                    if (player.Party is null)
                    {
                        reason = PortalTeleportNotAllowedReason.OnlyForParty;
                        return false;
                    }

                    if (player.Party != null && (player.Party.Members.Count < destinationMapDef.MinMembersCount || (destinationMapDef.MaxMembersCount != 0 && player.Party.Members.Count > destinationMapDef.MaxMembersCount)))
                    {
                        reason = PortalTeleportNotAllowedReason.NotEnoughPartyMembers;
                        return false;
                    }

                    return true;
                }

                // TODO: implement this check as soon as we have gulds.
                if (destinationMapDef.CreateType == CreateType.Guild /*&& player.Guild is null*/)
                {
                    reason = PortalTeleportNotAllowedReason.OnlyForGuilds;
                    return false;
                }

                if (!destinationMapDef.IsOpen)
                {
                    reason = PortalTeleportNotAllowedReason.OnlyForPartyAndOnTime;
                    return false;
                }

                return true;
            }
        }

        #endregion

        #region Party Maps

        /// <summary>
        /// Thread-safe dictionary of maps. Where key is party id.
        /// </summary>
        public ConcurrentDictionary<Guid, IPartyMap> PartyMaps { get; private set; } = new ConcurrentDictionary<Guid, IPartyMap>();

        private void PartyMap_OnAllMembersLeft(IPartyMap senser)
        {
            senser.OnAllMembersLeft -= PartyMap_OnAllMembersLeft;
            PartyMaps.TryRemove(senser.PartyId, out var removed);
        }

        #endregion

        #region Players

        /// <inheritdoc />
        public ConcurrentDictionary<int, Character> Players { get; private set; } = new ConcurrentDictionary<int, Character>();

        public ConcurrentDictionary<int, TradeManager> TradeManagers { get; private set; } = new ConcurrentDictionary<int, TradeManager>();

        public ConcurrentDictionary<int, PartyManager> PartyManagers { get; private set; } = new ConcurrentDictionary<int, PartyManager>();

        public ConcurrentDictionary<int, DuelManager> DuelManagers { get; private set; } = new ConcurrentDictionary<int, DuelManager>();

        /// <inheritdoc />
        public async Task<Character> LoadPlayer(int characterId, WorldClient client)
        {
            var newPlayer = await _characterFactory.CreateCharacter(characterId, client);
            if (newPlayer is null)
                return null;

            Players.TryAdd(newPlayer.Id, newPlayer);
            TradeManagers.TryAdd(newPlayer.Id, new TradeManager(this, newPlayer));
            PartyManagers.TryAdd(newPlayer.Id, new PartyManager(this, newPlayer));
            DuelManagers.TryAdd(newPlayer.Id, new DuelManager(this, newPlayer));

            _logger.LogDebug($"Player {newPlayer.Id} connected to game world");
            newPlayer.Client.OnPacketArrived += Client_OnPacketArrived;

            return newPlayer;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case CharacterEnteredMapPacket enteredMapPacket:
                    LoadPlayerInMap(((WorldClient)sender).CharID);
                    break;
            }

        }

        /// <inheritdoc />
        public void LoadPlayerInMap(int characterId)
        {
            var player = Players[characterId];
            if (Maps.ContainsKey(player.MapId))
            {
                Maps[player.MapId].LoadPlayer(player);
            }
            else
            {
                var mapDef = _mapDefinitions.Maps.FirstOrDefault(d => d.Id == player.MapId);

                // Map is not found.
                if (mapDef is null)
                {
                    _logger.LogWarning($"Unknown map {player.MapId} for character {player.Id}. Fallback to 0 map.");
                    var town = Maps[0].GetNearestSpawn(player.PosX, player.PosY, player.PosZ, player.Country);
                    player.Teleport(0, town.X, town.Y, town.Z);
                    return;
                }

                if (mapDef.CreateType == CreateType.Party)
                {
                    if (player.Party != null)
                    {
                        PartyMaps.TryGetValue(player.Party.Id, out var map);
                        if (map is null)
                        {
                            map = _mapFactory.CreatePartyMap(mapDef.Id, mapDef, _mapsLoader.LoadMapConfiguration(mapDef.Id), player.Party);
                            map.OnAllMembersLeft += PartyMap_OnAllMembersLeft;
                            PartyMaps.TryAdd(player.Party.Id, map);
                        }

                        map.LoadPlayer(player);
                    }
                    else // Map is for party, and player is not in party. Load player to nearest to this map.
                    {
                        // TODO: load to near to intance map.
                    }
                }
            }
        }

        /// <inheritdoc />
        public void RemovePlayer(int characterId)
        {
            Character player;
            if (Players.TryRemove(characterId, out player))
            {
                _logger.LogDebug($"Player {characterId} left game world");

                TradeManagers.TryRemove(characterId, out var tradeManager);
                tradeManager.Dispose();

                PartyManagers.TryRemove(characterId, out var partyManager);
                partyManager.Dispose();

                DuelManagers.TryRemove(characterId, out var duelManager);
                duelManager.Dispose();

                player.Client.OnPacketArrived -= Client_OnPacketArrived;

                var map = Maps[player.MapId];
                map.UnloadPlayer(player);
                player.Dispose();
            }
            else
            {
                // 0 means, that connection with client was lost, when he was in character selection screen.
                if (characterId != 0)
                {
                    _logger.LogError($"Couldn't remove player {characterId} from game world");
                }
            }

        }

        #endregion


        #region Bless

        /// <summary>
        /// Goddess bless.
        /// </summary>
        public Bless Bless { get; private set; } = Bless.Instance;

        #endregion
    }
}
