using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System;

namespace Imgeneus.Network.Packets.Login
{
    public struct SelectServerPacket : IEquatable<SelectServerPacket>, IDeserializedPacket
    {
        public byte WorldId { get; }

        public int BuildClient { get; }

        public SelectServerPacket(IPacketStream packet)
        {
            this.WorldId = packet.Read<byte>();
            this.BuildClient = packet.Read<int>();
        }

        public bool Equals(SelectServerPacket other)
        {
            return this.WorldId == other.WorldId &&
                this.BuildClient == other.BuildClient;
        }
    }
}
