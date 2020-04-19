using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Network.Client;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Packets.InternalServer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using static Imgeneus.Network.Server.IServerClient;

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

        public event Action<IDeserializedPacket> OnPacketArrived;

        public override void HandlePacket(IPacketStream packet)
        {
            PacketDeserializeHandler handler;

            if (PacketHandlers.TryGetValue(packet.PacketType, out handler))
            {
                var deserializedPacket = handler.Invoke(packet);
                OnPacketArrived?.Invoke(deserializedPacket);
            }
            else
            {
                if (Enum.IsDefined(typeof(PacketType), packet.PacketType))
                {
                    this.logger.LogWarning("Received an unimplemented packet {0}.", packet.PacketType);
                }
                else
                {
                    this.logger.LogWarning("Received an unknown packet 0x{0}.", ((ushort)packet.PacketType).ToString("X2"));
                }
            }
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

        /// <summary>
        /// All packets transformations available for internal server.
        /// </summary>
        private readonly Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers = new Dictionary<PacketType, PacketDeserializeHandler>()
        {
             { PacketType.AES_KEY_RESPONSE, (s) => new AesKeyResponsePacket(s) },
        };
    }
}
