using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Server;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Player;
using Imgeneus.World.InternalServer;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Imgeneus.World
{
    public sealed class WorldServer : Server<WorldClient>, IWorldServer
    {
        private readonly ILogger<WorldServer> _logger;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly IGameWorld _gameWorld;

        /// <summary>
        /// Gets the Inter-Server client.
        /// </summary>
        public ISClient InterClient { get; private set; }

        public WorldServer(ILogger<WorldServer> logger, WorldConfiguration configuration, IGameWorld gameWorld)
            : base(new ServerConfiguration(configuration.Host, configuration.Port, configuration.MaximumNumberOfConnections))
        {
            _logger = logger;
            _worldConfiguration = configuration;
            _gameWorld = gameWorld;
            InterClient = new ISClient(configuration);

            ConnectToGameWorld();
        }

        private void ConnectToGameWorld()
        {
            _gameWorld.OnPlayerEnteredMap += GameWorld_OnPlayerConnected;
            _gameWorld.OnPlayerMove += GameWorld_OnPlayerMove;
            _gameWorld.OnPlayerMotion += GameWorld_OnPlayerMotion;
        }

        private void GameWorld_OnPlayerMotion(int characterId, Motion motion)
        {
            // Send notification each player about motion.
            foreach (var client in clients)
            {
                WorldPacketFactory.CharacterMotion(client.Value, characterId, motion);
            }
        }

        private void GameWorld_OnPlayerConnected(Character newPlayer)
        {
            // Send other clients notification, that user is connected to game.
            foreach (var client in clients.Where(c => c.Value.CharID != 0 && c.Value.CharID != newPlayer.Id))
            {
                WorldPacketFactory.CharacterConnectedToMap(client.Value, newPlayer);
            }
        }

        private void GameWorld_OnPlayerMove(Character character)
        {
            // Send other clients notification, that user is moving.
            foreach (var client in clients.Where(c => c.Value.CharID != 0 && c.Value.CharID != character.Id))
            {
                WorldPacketFactory.CharacterMoves(client.Value, character);
            }
        }

        protected override void OnStart()
        {
            _logger.LogTrace("Host: {0}, Port: {1}, MaxNumberOfConnections: {2}",
                _worldConfiguration.Host,
                _worldConfiguration.Port,
                _worldConfiguration.MaximumNumberOfConnections);
            InterClient.Connect();
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            _logger.LogInformation($"World Server error: {exception.Message}");
        }
    }
}
