using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RaidCreatePacket : IDeserializedPacket
    {
        public bool RaidType;

        public bool AutoJoin;

        public int DropType;

        public RaidCreatePacket(IPacketStream packet)
        {
            RaidType = packet.Read<bool>();
            AutoJoin = packet.Read<bool>();
            DropType = packet.Read<int>();
        }
    }
}
