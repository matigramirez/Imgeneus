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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
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
            var mapDefinitions = _mapsLoader.LoadMapDefinitions();
            foreach (var mapDefinition in mapDefinitions.Maps)
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
            Maps[player.MapId].LoadPlayer(player);
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
