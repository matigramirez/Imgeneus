using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Trade;
using Imgeneus.World.Game.Zone;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public class GameWorld : IGameWorld
    {
        private readonly ILogger<GameWorld> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IDatabasePreloader _databasePreloader;

        public GameWorld(ILogger<GameWorld> logger, IBackgroundTaskQueue taskQueue, IDatabasePreloader databasePreloader)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _databasePreloader = databasePreloader;

            InitMaps();
        }

        #region Maps 

        /// <summary>
        /// Thread-safe dictionary of maps. Where key is map id.
        /// </summary>
        public ConcurrentDictionary<ushort, Map> Maps { get; private set; } = new ConcurrentDictionary<ushort, Map>();

        /// <summary>
        /// Initializes maps with startup values like mobs, npc, areas, obelisks etc.
        /// </summary>
        private void InitMaps()
        {
            // TODO: init maps here. For now create 0-map(DWaterBorderland, Lvl 40-80)
            Maps.TryAdd(0, new Map(0, DependencyContainer.Instance.Resolve<ILogger<Map>>()));
        }

        #endregion

        #region Players

        /// <inheritdoc />
        public ConcurrentDictionary<int, Character> Players { get; private set; } = new ConcurrentDictionary<int, Character>();

        public ConcurrentDictionary<int, TradeManager> TradeManagers { get; private set; } = new ConcurrentDictionary<int, TradeManager>();

        /// <inheritdoc />
        public Character LoadPlayer(int characterId, WorldClient client)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbCharacter = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                               .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                               .Include(c => c.ActiveBuffs).ThenInclude(cb => cb.Skill)
                                               .Include(c => c.QuickItems)
                                               .Include(c => c.User)
                                               .FirstOrDefault(c => c.Id == characterId);
            var newPlayer = Character.FromDbCharacter(dbCharacter, DependencyContainer.Instance.Resolve<ILogger<Character>>(), _taskQueue, _databasePreloader);
            newPlayer.Client = client;

            Players.TryAdd(newPlayer.Id, newPlayer);
            TradeManagers.TryAdd(newPlayer.Id, new TradeManager(this, newPlayer));

            _logger.LogDebug($"Player {newPlayer.Id} connected to game world");
            newPlayer.Client.OnPacketArrived += Client_OnPacketArrived;

            return newPlayer;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            if (packet is CharacterEnteredMapPacket)
            {
                LoadPlayerInMap(((WorldClient)sender).CharID);
            }
        }

        /// <inheritdoc />
        public void LoadPlayerInMap(int characterId)
        {
            var player = Players[characterId];
            Maps[player.Map].LoadPlayer(player);
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

                player.Client.OnPacketArrived -= Client_OnPacketArrived;

                var map = Maps[player.Map];
                map.UnloadPlayer(player);
                player.ClearConnection();
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
    }
}
