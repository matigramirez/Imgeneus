using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildJoinResultPacket : IDeserializedPacket
    {
        public bool Ok { get; }

        public int CharacterId { get; }

        public GuildJoinResultPacket(IPacketStream packet)
        {
            Ok = packet.Read<bool>();
            CharacterId = packet.Read<int>();
        }
    }
}
