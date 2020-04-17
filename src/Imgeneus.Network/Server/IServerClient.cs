using Imgeneus.Network.Common;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Game;
using System;
using System.Collections.Generic;

namespace Imgeneus.Network.Server
{
    public interface IServerClient : IConnection, IDisposable
    {
        /// <summary>
        /// Gets the client's parent server.
        /// </summary>
        IServer Server { get; }

        /// <summary>
        /// Gets the remote end point (IP and port) for this client.
        /// </summary>
        string RemoteEndPoint { get; }

        /// <summary>
        /// Disconnects the current <see cref="IServerClient"/>.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Handle an incoming packet.
        /// </summary>
        /// <param name="packet">The incoming packet.</param>
        void HandlePacket(IPacketStream packet);

        /// <summary>
        /// Sends a packet to this client.
        /// </summary>
        /// <param name="packet">packet to send</param>
        /// <param name="shouldEncrypt">flag, that indicates if the packet should be encrypted. By default any packet is encrypted.</param>
        void SendPacket(IPacketStream packet, bool shouldEncrypt = true);

        /// <summary>
        /// Delegate, that return deserialized packet.
        /// </summary>
        public delegate IDeserializedPacket PacketDeserializeHandler(IPacketStream stream);

        /// <summary>
        /// This dictionary contains inofmation how oacket stream should be transformed based on packet type.
        /// </summary>
        Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers { get; }
    }
}
