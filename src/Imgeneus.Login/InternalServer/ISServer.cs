using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Login.InternalServer.Packets;
using Imgeneus.Network.InternalServer;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Packets.InternalServer;
using Imgeneus.Network.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.Login.InternalServer
{
    public sealed class ISServer : Server<ISClient>
    {
        private readonly ILogger<ISServer> logger;

        public ISServer(InterServerConfiguration configuration) : base(configuration.Host, configuration.Port, 5)
        {
            this.logger = DependencyContainer.Instance.Resolve<ILogger<ISServer>>();
        }

        protected override void OnStart()
        {
            this.logger.LogInformation("ISC server is started and listen on {0}:{1}.",
            this.ServerConfiguration.Host,
            this.ServerConfiguration.Port);
            this.logger.LogTrace("Inter-Server -> Host: {0}, Port: {1}, MaxNumberOfConnections: {2}",
            this.ServerConfiguration.Host,
            this.ServerConfiguration.Port,
            this.ServerConfiguration.MaximumNumberOfConnections);
        }

        protected override void OnClientConnected(ISClient client)
        {
            this.logger.LogTrace("New world server connected from {0}", client.RemoteEndPoint);

            client.OnPacketArrived += Client_OnPacketArrived;
        }

        protected override void OnClientDisconnected(ISClient client)
        {
            this.logger.LogTrace("World server disconnected");

            client.OnPacketArrived -= Client_OnPacketArrived;
        }

        protected override void OnError(Exception exception)
        {
            this.logger.LogInformation($"Internal-Server socket error: {exception.Message}");
        }

        /// <summary>
        /// Gets the list of the connected worlds.
        /// </summary>
        public IEnumerable<WorldServerInfo> WorldServers => this.clients.Values.Select(x => x.WorldServerInfo);


        /// <summary>
        /// Gets a world server by id.
        /// </summary>
        public WorldServerInfo GetWorldServerByID(byte id)
            => this.clients.Values.Select(x => x.WorldServerInfo).FirstOrDefault(x => x.Id == id);

        public ISClient GetWorldClientByID(byte id)
            => this.clients.Values.FirstOrDefault(x => x.WorldServerInfo.Id == id);

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            if (packet is AuthenticateServerPacket)
            {
                var authPacket = (AuthenticateServerPacket)packet;
                (sender as ISClient).SetWordServerInfo(authPacket.WorldServerInfo);
            }

            // World serber requests keys of loginc client.
            if (packet is AesKeyRequestPacket)
            {
                var aesRequestPacket = (AesKeyRequestPacket)packet;
                LoginClients.TryGetValue(aesRequestPacket.Guid, out var loginClient);
                ISPacketFactory.SendAuthentication(sender, loginClient);

                // Remove login client as soon as it's sent. We don't need it anymore.
                LoginClients.TryRemove(loginClient.Id, out var removed);
            }
        }

        /// <summary>
        /// This dictionary is needed for storing login clients.
        /// As soon as world server requests aes key and iv, we should send answer. That's why we're storing login clients.
        /// </summary>
        public readonly ConcurrentDictionary<Guid, LoginClient> LoginClients = new ConcurrentDictionary<Guid, LoginClient>();
    }
}
