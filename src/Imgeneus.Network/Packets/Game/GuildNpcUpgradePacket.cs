using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GuildNpcUpgradePacket : IDeserializedPacket
    {
        public byte NpcType { get; }

        public byte NpcGroup { get; }

        public byte NpcLevel { get; }

        public GuildNpcUpgradePacket(IPacketStream packet)
        {
            NpcType = packet.Read<byte>();
            NpcGroup = packet.Read<byte>();
            NpcLevel = packet.Read<byte>();
        }
    }
}
