using Imgeneus.Network.Data;
using Imgeneus.Network.InternalServer;
using Imgeneus.Network.Packets.Game;

namespace Imgeneus.Network.Packets.InternalServer
{
    public struct AuthenticateServerPacket : IDeserializedPacket
    {
        public WorldServerInfo WorldServerInfo { get; }

        public AuthenticateServerPacket(IPacketStream packet)
        {
            byte id = 1;
            byte[] host = packet.Read<byte>(4);
            string name = packet.ReadString(32);
            int buildVersion = packet.Read<int>();
            ushort maxConnections = packet.Read<ushort>();

            WorldServerInfo = new WorldServerInfo(id, host, name, buildVersion, maxConnections);
        }
    }
}
