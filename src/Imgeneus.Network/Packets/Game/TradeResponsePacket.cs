using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeResponsePacket : IDeserializedPacket
    {
        public bool IsDeclined { get; }

        public TradeResponsePacket(IPacketStream packet)
        {
            IsDeclined = packet.Read<bool>();
        }
    }
}
