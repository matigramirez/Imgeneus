using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct UpdateStatsPacket : IDeserializedPacket
    {
        public ushort Str;
        public ushort Dex;
        public ushort Rec;
        public ushort Int;
        public ushort Wis;
        public ushort Luc;

        public UpdateStatsPacket(IPacketStream packet)
        {
            Str = packet.Read<ushort>();
            Dex = packet.Read<ushort>();
            Rec = packet.Read<ushort>();
            Int = packet.Read<ushort>();
            Wis = packet.Read<ushort>();
            Luc = packet.Read<ushort>();
        }
    }
}
