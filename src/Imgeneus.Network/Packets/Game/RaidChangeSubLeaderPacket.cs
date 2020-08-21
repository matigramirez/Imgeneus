using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidChangeSubLeaderPacket : IDeserializedPacket
    {
        public int CharacterId;

        public RaidChangeSubLeaderPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
