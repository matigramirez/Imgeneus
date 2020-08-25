using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct NpcBuyItemPacket : IDeserializedPacket
    {
        public int NpcId;

        public byte ItemIndex;

        public byte Count;

        public NpcBuyItemPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
            ItemIndex = packet.Read<byte>();
            Count = packet.Read<byte>();
        }
    }
}
