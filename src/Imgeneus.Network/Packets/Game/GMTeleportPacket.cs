using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMTeleportPacket : IDeserializedPacket
    {
        public float X;

        public float Z;

        public ushort MapId;

        public GMTeleportPacket(IPacketStream packet)
        {
            X = packet.Read<float>();
            Z = packet.Read<float>();
            MapId = packet.Read<ushort>();
        }
    }
}
