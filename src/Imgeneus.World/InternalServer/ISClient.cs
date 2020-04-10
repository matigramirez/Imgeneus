using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Network.Client;
using Imgeneus.Network.Data;
using Microsoft.Extensions.Logging;
using System;

namespace Imgeneus.World.InternalServer
{
    public sealed class ISClient : Client
    {
        private readonly ILogger<ISClient> logger;

        public ISClient(WorldConfiguration worldConfiguration)
            : base(new ClientConfiguration(worldConfiguration.InterServerConfiguration.Host, worldConfiguration.InterServerConfiguration.Port))
        {
            this.WorldConfiguration = worldConfiguration;
            this.logger = DependencyContainer.Instance.Resolve<ILogger<ISClient>>();
        }

        /// <summary>
        /// Gets the world server's configuration.
        /// </summary>
        public WorldConfiguration WorldConfiguration { get; }

        public override void HandlePacket(IPacketStream packet)
        {
            throw new NotImplementedException();
        }

        protected override void OnConnected()
        {
            this.logger.LogInformation("Inter-Server connected to Login Server");
            ISPacketFactory.Authenticate(this, WorldConfiguration);
        }

        protected override void OnDisconnected()
        {
            this.logger.LogInformation("Inter-Server disconnected from Login Server");
        }

        protected override void OnError(Exception exception)
        {
            this.logger.LogError($"Internal-Server socket error: {exception.Message}");
        }
    }
}
