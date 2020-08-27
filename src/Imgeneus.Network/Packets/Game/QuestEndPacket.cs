using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct QuestEndPacket : IDeserializedPacket
    {
        public int NpcId;

        public ushort QuestId;

        public QuestEndPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
            QuestId = packet.Read<ushort>();
        }
    }
}
