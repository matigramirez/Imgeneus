using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMTeleportPlayerPacket : IDeserializedPacket
    {
        public string Name;

        public float X;

        public float Z;

        public ushort MapId;

        public GMTeleportPlayerPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
            X = packet.Read<float>();
            Z = packet.Read<float>();
            MapId = packet.Read<ushort>();
        }

        public void Deconstruct(out string name, out float x, out float z, out ushort mapId)
        {
            name = Name;
            x = X;
            z = Z;
            mapId = MapId;
        }
    }
}