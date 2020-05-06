using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeFinishPacket : IDeserializedPacket
    {
        public byte Result;

        public TradeFinishPacket(IPacketStream packet)
        {
            Result = packet.Read<byte>();
        }
    }
}
