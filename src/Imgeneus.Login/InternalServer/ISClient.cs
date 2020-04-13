using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.InternalServer;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Imgeneus.Login.InternalServer
{
    public class ISClient : ServerClient
    {
        private readonly ILogger<ISClient> logger;

        /// <summary>
        /// The World server information.
        /// </summary>
        public WorldServerInfo WorldServerInfo { get; private set; }

        public ISClient(IServer server, Socket acceptedSocket)
            : base(server, acceptedSocket)
        {
            this.logger = DependencyContainer.Instance.Resolve<ILogger<ISClient>>();
        }

        public override void HandlePacket(IPacketStream packet)
        {
            if (this.Socket == null)
            {
                this.logger.LogTrace("Skip to handle packet from {0}. Reason: socket is no more connected.", this.RemoteEndPoint);
                return;
            }

            try
            {
                if (!PacketHandler<ISClient>.Invoke(this, packet, packet.PacketType))
                {
                    if (Enum.IsDefined(typeof(PacketType), packet.PacketType))
                    {
                        this.logger.LogWarning("Received an unimplemented packet {0} from {2}.", packet.PacketType, this.RemoteEndPoint);
                    }
                    else
                    {
                        this.logger.LogWarning("Received an unknown packet 0x{0} from {1}.", ((ushort)packet.PacketType).ToString("X2"), this.RemoteEndPoint);
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger.LogError("Packet handle error from {0}. {1}", this.RemoteEndPoint, exception.Message);
                this.logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }

        public void SetWordServerInfo(WorldServerInfo worldServerInfo)
        {
            if (this.WorldServerInfo != null)
            {
                throw new InvalidOperationException("Client world server info already set.");
            }
            this.WorldServerInfo = worldServerInfo;
        }

        /// <summary>
        /// PacketHandlers is replacement for PacketHandler<T>. Right now implemented only for world client.
        /// If I have some time and mood, I'll implement it for internal server too. Until then leave it unimplemented.
        /// Check how it's done im world server.
        /// </summary>
        public override Dictionary<PacketType, IServerClient.PacketDeserializeHandler> PacketHandlers => throw new NotImplementedException();
    }
}
