using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildKickPacket : IDeserializedPacket
    {
        public int CharacterId { get; }

        public GuildKickPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
