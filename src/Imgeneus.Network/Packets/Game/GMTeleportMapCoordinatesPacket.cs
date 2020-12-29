using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMTeleportMapCoordinatesPacket : IDeserializedPacket
    {
        public float X;

        public float Z;

        public ushort MapId;

        public GMTeleportMapCoordinatesPacket(IPacketStream packet)
        {
            X = packet.Read<float>();
            Z = packet.Read<float>();
            MapId = packet.Read<ushort>();
        }

        public void Deconstruct(out float x, out float z, out ushort mapId)
        {
            x = X;
            z = Z;
            mapId = MapId;
        }
    }
}
