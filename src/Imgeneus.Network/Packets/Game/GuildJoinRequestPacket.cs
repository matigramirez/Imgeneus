using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildJoinRequestPacket : IDeserializedPacket
    {
        public int GuildId { get; }

        public GuildJoinRequestPacket(IPacketStream packet)
        {
            GuildId = packet.Read<int>();
        }
    }
}
