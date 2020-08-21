using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidChangeLeaderPacket : IDeserializedPacket
    {
        public int CharacterId;

        public RaidChangeLeaderPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
