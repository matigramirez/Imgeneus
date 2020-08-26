using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct QuestStartPacket : IDeserializedPacket
    {
        public int NpcId;

        public ushort QuestId;

        public QuestStartPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
            QuestId = packet.Read<ushort>();
        }
    }
}
