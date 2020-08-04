using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DuelRequestPacket : IDeserializedPacket
    {
        public int DuelToWhomId { get; }

        public DuelRequestPacket(IPacketStream packet)
        {
            DuelToWhomId = packet.Read<int>();
        }
    }
}
