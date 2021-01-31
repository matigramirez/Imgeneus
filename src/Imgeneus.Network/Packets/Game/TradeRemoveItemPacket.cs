using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeRemoveItemPacket : IDeserializedPacket
    {
        public byte SlotInTradeWindow;

        public TradeRemoveItemPacket(IPacketStream packet)
        {
            SlotInTradeWindow = packet.Read<byte>();
        }
    }
}
