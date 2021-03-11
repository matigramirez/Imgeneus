using Imgeneus.Core.DependencyInjection;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Packets.Login;
using Imgeneus.Network.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Imgeneus.Network.Server.IServerClient;

namespace Imgeneus.Login
{
    public sealed class LoginClient : ServerClient
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Gets the client's logged user id.
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Check if the client is connected.
        /// </summary>
        public bool IsConnected => this.UserId != 0;

        /// <inheritdoc />
        public override event Action<ServerClient, IDeserializedPacket> OnPacketArrived;

        /// <summary>
        /// Creates a new <see cref="LoginClient"/> instance.
        /// </summary>
        /// <param name="server">The parent login server.</param>
        /// <param name="socket">The accepted socket.</param>
        public LoginClient(ILoginServer server, Socket socket, ILogger logger)
            : base(server, socket)
        {
            _logger = logger;
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
                _logger.LogTrace("Skip to handle packet. Reason: client is no more connected.");
                return;
            }

            try
            {
                PacketDeserializeHandler handler;

                if (PacketHandlers.TryGetValue(packet.PacketType, out handler))
                {
                    var deserializedPacket = handler.Invoke(packet);
                    OnPacketArrived?.Invoke(this, deserializedPacket);
                }
                else
                {
                    if (Enum.IsDefined(typeof(PacketType), packet.PacketType))
                    {
                        _logger.LogWarning("Received an unimplemented packet {0} from {1}.", packet.PacketType, this.RemoteEndPoint);
                    }
                    else
                    {
                        _logger.LogWarning("Received an unknown packet 0x{0} from {1}.", ((ushort)packet.PacketType).ToString("X2"), this.RemoteEndPoint);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Packet handle error from {0}. {1}", this.RemoteEndPoint, exception.Message);
                _logger.LogDebug(exception.InnerException?.StackTrace);
            }
        }

        private readonly Dictionary<PacketType, PacketDeserializeHandler> _packetHandlers = new Dictionary<PacketType, PacketDeserializeHandler>()
        {
            { PacketType.LOGIN_HANDSHAKE, (s) => new LoginHandshakePacket(s) },
            { PacketType.LOGIN_REQUEST, (s) => new AuthenticationPacket(s) },
            { PacketType.OAUTH_LOGIN_REQUEST, (s) => new OAuthAuthenticationPacket(s) },
            { PacketType.SELECT_SERVER, (s) =>  new SelectServerPacket(s) }
        };

        /// <summary>
        /// All available packet handlers for login client.
        /// </summary>
        public override Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers => _packetHandlers;
    }
}
