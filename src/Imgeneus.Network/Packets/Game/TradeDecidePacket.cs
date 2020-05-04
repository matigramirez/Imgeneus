using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeDecidePacket : IDeserializedPacket
    {
        public bool IsDecided { get; }

        public TradeDecidePacket(IPacketStream packet)
        {
            IsDecided = packet.Read<bool>();
        }
    }
}
