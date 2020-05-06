using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct TradeAddMoneyPacket : IDeserializedPacket
    {
        public uint Money { get; }

        public TradeAddMoneyPacket(IPacketStream packet)
        {
            Money = packet.Read<uint>();
        }
    }
}
