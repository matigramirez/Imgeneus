using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelAddMoneyPacket : IDeserializedPacket
    {
        public uint Money { get; }

        public DuelAddMoneyPacket(IPacketStream packet)
        {
            Money = packet.Read<uint>();
        }
    }
}
