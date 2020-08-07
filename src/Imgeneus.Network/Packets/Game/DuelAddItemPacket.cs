using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelAddItemPacket : IDeserializedPacket
    {
        public byte Bag;

        public byte Slot;

        public byte Quantity;

        public byte SlotInTradeWindow;

        public DuelAddItemPacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
            Quantity = packet.Read<byte>();
            SlotInTradeWindow = packet.Read<byte>();
        }

    }
}
