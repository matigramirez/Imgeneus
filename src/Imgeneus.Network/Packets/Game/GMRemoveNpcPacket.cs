using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMRemoveNpcPacket : IDeserializedPacket
    {
        public byte Type;

        public ushort TypeId;

        public byte Count;

        public GMRemoveNpcPacket(IPacketStream packet)
        {
            Type = packet.Read<byte>();
            TypeId = packet.Read<ushort>();
            Count = packet.Read<byte>();
        }
    }
}
