using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Network.Client;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using System.Net;

namespace Imgeneus.World.InternalServer
{
    public static class ISPacketFactory
    {
        public static void Authenticate(IClient client)
        {
            using var packet = new Packet(PacketType.AUTH_SERVER);

            packet.Write<byte[]>(IPAddress.Parse("127.0.0.1").GetAddressBytes());
            packet.WriteString("Imgeneus", 32);
            packet.Write<int>(0);
            packet.Write<ushort>(1000);

            client.SendPacket(packet);
        }
    }
}
