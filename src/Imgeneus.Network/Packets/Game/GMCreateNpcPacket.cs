using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMCreateNpcPacket : IDeserializedPacket
    {
        public byte Type;

        public ushort TypeId;

        public byte Count;

        public GMCreateNpcPacket(IPacketStream packet)
        {
            Type = packet.Read<byte>();
            TypeId = packet.Read<ushort>();
            Count = packet.Read<byte>();
        }
    }
}
