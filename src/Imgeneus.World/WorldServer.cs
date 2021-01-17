using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Server;
using Imgeneus.Network.Server.Crypto;
using Imgeneus.World.Game;
using Imgeneus.World.SelectionScreen;
using InterServer.Client;
using InterServer.Common;
using InterServer.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imgeneus.World
{
    public sealed class WorldServer : Server<WorldClient>, IWorldServer
    {
        private readonly ILogger<WorldServer> _logger;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly IGameWorld _gameWorld;
        private readonly IInterServerClient _interClient;
        private readonly ISelectionScreenFactory _selectionScreenFactory;

        public WorldServer(ILogger<WorldServer> logger, IOptions<WorldConfiguration> configuration, IGameWorld gameWorld, IInterServerClient interClient, ISelectionScreenFactory selectionScreenFactory)
            : base(new ServerConfiguration(configuration.Value.Host, configuration.Value.Port, configuration.Value.MaximumNumberOfConnections), logger)
        {
            _logger = logger;
            _worldConfiguration = configuration.Value;
            _gameWorld = gameWorld;
            _interClient = interClient;
            _selectionScreenFactory = selectionScreenFactory;
        }

        public override void Start()
        {
            try
            {
                _logger.LogInformation("Starting WorldServer...");
                base.Start();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"An unexpected error occured in WorldServer.");
            }
        }

        protected override void OnStart()
        {
            _logger.LogTrace("Host: {0}, Port: {1}, MaxNumberOfConnections: {2}",
                _worldConfiguration.Host,
                _worldConfiguration.Port,
                _worldConfiguration.MaximumNumberOfConnections);
            _interClient.Connect();
            _interClient.OnConnected += SendWorldInfo;
        }

        private void SendWorldInfo()
        {
            _interClient.OnConnected -= SendWorldInfo;
            _interClient.Send(new ISMessage(ISMessageType.WORLD_INFO, _worldConfiguration));

            _interClient.OnSessionResponse += LoadSelectionScreen;
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

            client.OnPacketArrived += Client_OnPacketArrived;
        }

        private void LoadSelectionScreen(SessionResponse sessionInfo)
        {
            clients.TryGetValue(sessionInfo.SessionId, out var worldClient);
            worldClient.CryptoManager.GenerateAES(sessionInfo.KeyPair.Key, sessionInfo.KeyPair.IV);

            using var sendPacket = new Packet(PacketType.GAME_HANDSHAKE);
            sendPacket.WriteByte(0); // 0 means there was no error.
            sendPacket.WriteByte(2); // no idea what is it, it just works.
            sendPacket.Write(CryptoManager.XorKey);
            worldClient.SendPacket(sendPacket);

            SelectionScreenManagers[worldClient.Id].SendSelectionScrenInformation(worldClient.UserID);
        }

        private async void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            if (packet is HandshakePacket)
            {
                var handshake = (HandshakePacket)packet;
                (sender as WorldClient).SetClientUserID(handshake.UserId);

                clients.TryRemove(sender.Id, out var client);

                // Now give client new id.
                client.Id = handshake.SessionId;

                // Return client back to dictionary.
                clients.TryAdd(client.Id, client);
                SelectionScreenManagers.Add(client.Id, _selectionScreenFactory.CreateSelectionManager(client));

                // Send request to login server and get client key.
                await _interClient.Send(new ISMessage(ISMessageType.AES_KEY_REQUEST, sender.Id));
            }

            if (packet is PingPacket)
            {
                // TODO: implement disconnect, if client is not sending ping packet.
            }

            if (packet is CashPointPacket)
            {
                // TODO: implement cash point packet.
                using var dummyPacket = new Packet(PacketType.CASH_POINT);
                dummyPacket.Write(0);
                sender.SendPacket(dummyPacket);
            }

            if (packet is LogOutPacket)
            {
                // TODO: For sure, here should be timer!
                await Task.Delay(1000 * 10); // 10 seconds * 1000 milliseconds

                if (sender.IsDispose)
                    return;

                using var logoutPacket = new Packet(PacketType.LOGOUT);
                sender.SendPacket(logoutPacket);

                sender.CryptoManager.UseExpandedKey = false;

                if (SelectionScreenManagers.ContainsKey(sender.Id))
                    SelectionScreenManagers[sender.Id].SendSelectionScrenInformation(((WorldClient)sender).UserID);
            }
        }

        #region Screen selection

        /// <summary>
        /// Screen selection manager helps with packets, that must be sent right after gameshake.
        /// </summary>
        private readonly Dictionary<Guid, ISelectionScreenManager> SelectionScreenManagers = new Dictionary<Guid, ISelectionScreenManager>();

        #endregion
    }
}
