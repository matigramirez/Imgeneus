using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct QuestQuitPacket : IDeserializedPacket
    {
        public ushort QuestId;

        public QuestQuitPacket(IPacketStream packet)
        {
            QuestId = packet.Read<ushort>();
        }
    }
}
