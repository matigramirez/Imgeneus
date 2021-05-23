using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildEtinReturnPacket : IDeserializedPacket
    {
        public int NpcId;

        public GuildEtinReturnPacket(IPacketStream packet)
        {
            NpcId = packet.Read<int>();
        }
    }
}
