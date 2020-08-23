using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidMovePlayerPacket : IDeserializedPacket
    {
        public int SourceIndex;

        public int DestinationIndex;

        public RaidMovePlayerPacket(IPacketStream packet)
        {
            SourceIndex = packet.Read<int>();
            DestinationIndex = packet.Read<int>();
        }
    }
}
