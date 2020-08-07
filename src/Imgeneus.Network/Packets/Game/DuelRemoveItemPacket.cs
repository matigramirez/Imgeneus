using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelRemoveItemPacket : IDeserializedPacket
    {
        public byte SlotInTradeWindow;

        public DuelRemoveItemPacket(IPacketStream packet)
        {
            SlotInTradeWindow = packet.Read<byte>();
        }
    }
}
