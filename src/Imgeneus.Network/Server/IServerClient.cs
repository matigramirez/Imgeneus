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
        /// <param name="packet"></param>
        void SendPacket(IPacketStream packet);

        /// <summary>
        /// Sends a packet to an other client.
        /// </summary>
        /// <param name="client">Other client</param>
        /// <param name="packet">Packet</param>
        void SendPacketTo(IServerClient client, IPacketStream packet);

        /// <summary>
        /// Sends a packet to collection of clients.
        /// </summary>
        /// <param name="clients">Collection of clients</param>
        /// <param name="packet">Packet</param>
        void SendPacketTo(IEnumerable<IServerClient> clients, IPacketStream packet);

        /// <summary>
        /// Sends a packet to every connected clients on the server.
        /// </summary>
        /// <param name="packet">Packet</param>
        void SendPacketToAll(IPacketStream packet);

        public delegate IDeserializedPacket PacketDeserializeHandler(IPacketStream stream);

        /// <summary>
        /// This dictionary contains inofmation how oacket stream should be transformed based on packet type.
        /// </summary>
        Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers { get; }
    }
}
