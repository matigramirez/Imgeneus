using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildUserStatePacket : IDeserializedPacket
    {
        public bool Demote { get; }

        public int CharacterId { get; }

        public GuildUserStatePacket(IPacketStream packet)
        {
            Demote = packet.Read<bool>();
            CharacterId = packet.Read<int>();
        }
    }
}
