using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct AutoStatsSettingsPacket : IDeserializedPacket
    {
        public byte Str;

        public byte Dex;

        public byte Rec;

        public byte Int;

        public byte Wis;

        public byte Luc;

        public AutoStatsSettingsPacket(IPacketStream packet)
        {
            var unknown = packet.Read<uint>(); // probably character id.

            Str = packet.Read<byte>();
            Dex = packet.Read<byte>();
            Rec = packet.Read<byte>();
            Int = packet.Read<byte>();
            Luc = packet.Read<byte>();
            Wis = packet.Read<byte>();
        }
    }
}
