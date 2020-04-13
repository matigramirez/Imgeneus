using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.World.Game;
using Imgeneus.World.InternalServer;
using Imgeneus.World.SelectionScreen;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

        /// <inheritdoc />
        protected override void OnClientDisconnected(WorldClient client)
        {
            base.OnClientDisconnected(client);

            SelectionScreenManagers.Remove(client.Id, out var manager);
            manager.Dispose();
            client.OnPacketArrived -= Client_OnPacketArrived;

            _gameWorld.RemovePlayer(client.CharID);
        }

        /// <inheritdoc />
        protected override void OnClientConnected(WorldClient client)
        {
            base.OnClientConnected(client);

            SelectionScreenManagers.Add(client.Id, new SelectionScreenManager(client));
            client.OnPacketArrived += Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(WorldClient sender, IDeserializedPacket packet)
        {
            if (packet is HandshakePacket)
            {
                var handshake = (HandshakePacket)packet;
                sender.SetClientUserID(handshake.UserId);

                // TODO: refactor it with packet encryption.
                using var sendPacket = new Packet(PacketType.GAME_HANDSHAKE);
                sendPacket.Write(0);
                sender.SendPacket(sendPacket);

                SelectionScreenManagers[sender.Id].AfterGameshake(handshake);
            }

            if (packet is PingPacket)
            {
                // TODO: implemen disconnect, if client is not sending ping packet.
            }
        }

        #region Screen selection

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<Guid, SelectionScreenManager> SelectionScreenManagers = new Dictionary<Guid, SelectionScreenManager>();

        #endregion
    }
}
