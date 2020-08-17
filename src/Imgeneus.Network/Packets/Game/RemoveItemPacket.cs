using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RemoveItemPacket : IDeserializedPacket
    {
        public byte Bag;

        public byte Slot;

        public byte Count;

        public RemoveItemPacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
            Count = packet.Read<byte>();
        }
    }
}
