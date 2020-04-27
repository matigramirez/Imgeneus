using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeRequestPacket : IDeserializedPacket
    {
        public int TradeToWhomId { get; }

        public TradeRequestPacket(IPacketStream packet)
        {
            TradeToWhomId = packet.Read<byte>();
        }
    }
}
