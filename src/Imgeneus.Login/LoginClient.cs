using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Imgeneus.Login
{
    public sealed class LoginClient : ServerClient
    {
        private readonly ILogger<LoginClient> logger;

        /// <summary>
        /// Gets the client's logged user id.
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Check if the client is connected.
        /// </summary>
        public bool IsConnected => this.UserId != 0;

        /// <summary>
        /// Creates a new <see cref="LoginClient"/> instance.
        /// </summary>
        /// <param name="server">The parent login server.</param>
        /// <param name="socket">The accepted socket.</param>
        public LoginClient(ILoginServer server, Socket socket)
            : base(server, socket)
        {
            this.logger = DependencyContainer.Instance.Resolve<ILogger<LoginClient>>();
        }

        /// <summary>
        /// Sets the client's user id.
        /// </summary>
        /// <param name="userID">The client user id.</param>
        public void SetClientUserID(int userID)
        {
            if (this.UserId != 0)
            {
                throw new InvalidOperationException("Client user ID already set.");
            }

            this.UserId = userID;
        }

        /// <inheritdoc />
        public override void HandlePacket(IPacketStream packet)
        {
            if (this.Socket == null)
            {
                this.logger.LogTrace("Skip to handle packet. Reason: client is no more connected.");
                return;
            }

            try
            {
                if (!PacketHandler<LoginClient>.Invoke(this, packet, packet.PacketType))
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

        /// <summary>
        /// PacketHandlers is replacement for PacketHandler<T>. Right now implemented only for world client.
        /// If I have some time and mood, I'll implement it for login server too. Until then leave it unimplemented.
        /// Check how it's done im world server.
        /// </summary>
        public override Dictionary<PacketType, IServerClient.PacketDeserializeHandler> PacketHandlers => throw new NotImplementedException();
    }
}
