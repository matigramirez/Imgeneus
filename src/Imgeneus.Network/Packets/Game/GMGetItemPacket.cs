using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMGetItemPacket : IDeserializedPacket
    {
        public byte Type { get; set; }

        public byte TypeId { get; set; }

        public byte Count { get; set; }

        public GMGetItemPacket(IPacketStream packet)
        {
            Type = packet.Read<byte>();
            TypeId = packet.Read<byte>();
            Count = packet.Read<byte>();
        }
    }
}
