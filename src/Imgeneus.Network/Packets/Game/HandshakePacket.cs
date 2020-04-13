using Imgeneus.Network.Data;
using System;

namespace Imgeneus.Network.Packets.Game
{
    public struct HandshakePacket : IEquatable<HandshakePacket>, IDeserializedPacket
    {
        public int UserId { get; }

        public Guid SessionId { get; }

        public HandshakePacket(IPacketStream packet)
        {
            this.UserId = packet.Read<int>();
            this.SessionId = new Guid(packet.Read<byte>(16));
        }

        public bool Equals(HandshakePacket other)
        {
            return this.UserId == other.UserId &&
                this.SessionId == other.SessionId;
        }
    }
}
