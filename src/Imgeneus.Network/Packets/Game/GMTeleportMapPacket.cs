using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMTeleportMapPacket : IDeserializedPacket
    {
        public ushort MapId;

        public GMTeleportMapPacket(IPacketStream packet)
        {
            MapId = packet.Read<ushort>();
        }
    }
}