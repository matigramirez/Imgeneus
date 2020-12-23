using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Login.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Packets.Login;
using Imgeneus.Network.Server;
using InterServer.Client;
using InterServer.Common;
using InterServer.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.Login
{
    public sealed class LoginServer : Server<LoginClient>, ILoginServer
    {
        private readonly ILogger<LoginServer> _logger;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly IInterServer _interServer;
        private readonly ILoginManagerFactory _loginManagerFactory;

        /// <summary>
        /// Gets the list of the connected worlds.
        /// </summary>
        public IEnumerable<WorldServerInfo> ClustersConnected => _interServer.WorldServers;

        public LoginServer(ILogger<LoginServer> logger, IOptions<LoginConfiguration> loginConfiguration, IInterServer interServer, ILoginManagerFactory loginManagerFactory)
            : base(new ServerConfiguration(loginConfiguration.Value.Host, loginConfiguration.Value.Port, loginConfiguration.Value.MaximumNumberOfConnections), logger)
        {
            _logger = logger;
            _loginConfiguration = loginConfiguration.Value;
            _interServer = interServer;
            _loginManagerFactory = loginManagerFactory;
        }

        public override void Start()
        {
            try
            {
                _logger.LogInformation("Starting LoginServer...");
                base.Start();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"An unexpected error occured in LoginServer.");
            }
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            this._logger.LogTrace("Host: {0}, Port: {1}, MaxNumberOfConnections: {2}",
            this._loginConfiguration.Host,
            this._loginConfiguration.Port,
            this._loginConfiguration.MaximumNumberOfConnections);
        }

        /// <summary>
        /// Login managers for each client.
        /// </summary>
        private readonly Dictionary<Guid, LoginManager> LoginManagers = new Dictionary<Guid, LoginManager>();

        /// <inheritdoc />
        protected override void OnClientConnected(LoginClient client)
        {
            base.OnClientConnected(client);

            LoginPacketFactory.SendLoginHandshake(client);

            LoginManagers.Add(client.Id, _loginManagerFactory.CreateLoginManager(client, this));
            client.OnPacketArrived += Client_OnPacketArrived;
        }

        protected override void OnClientDisconnected(LoginClient client)
        {
            base.OnClientDisconnected(client);

            LoginManagers.Remove(client.Id, out var manager);
            manager.Dispose();
            client.OnPacketArrived -= Client_OnPacketArrived;
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            if (packet is LoginHandshakePacket)
            {
                var handshakePacket = (LoginHandshakePacket)packet;
                var decryptedNumber = sender.CryptoManager.DecryptRSA(handshakePacket.EncyptedNumber);
                sender.CryptoManager.GenerateAES(decryptedNumber);

                _interServer.Sessions.TryAdd(sender.Id, new KeyPair(sender.CryptoManager.Key, sender.CryptoManager.IV));
            }
        }

        /// <inheritdoc />
        protected override void OnError(Exception exception)
        {
            this._logger.LogInformation($"Login Server error: {exception.Message}");
        }

        /// <inheritdoc />
        public IEnumerable<WorldServerInfo> GetConnectedWorlds() => _interServer.WorldServers;

        /// <inheritdoc />
        public LoginClient GetClientByUserID(int userID)
            => this.clients.Values.FirstOrDefault(x => x.IsConnected && x.UserId == userID);

        /// <inheritdoc />
        public bool IsClientConnected(int userID) => this.GetClientByUserID(userID) != null;

        /// <inheritdoc />
        public WorldServerInfo GetWorldByID(byte id) => _interServer.WorldServers.FirstOrDefault(x => x.Id == id);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
