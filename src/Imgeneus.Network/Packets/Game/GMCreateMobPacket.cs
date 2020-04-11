using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMCreateMobPacket
    {
        public ushort MobId { get; }

        public byte NumberOfMobs { get; }

        public GMCreateMobPacket(IPacketStream packet)
        {
            MobId = packet.Read<ushort>();
            NumberOfMobs = packet.Read<byte>();
        }
    }
}
