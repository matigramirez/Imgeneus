using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Server;

namespace Imgeneus.Login.InternalServer.Packets
{
    public static class ISPacketFactory
    {
        public static void SendAuthentication(IServerClient worldClient, LoginClient loginClient)
        {
            using var packet = new Packet(PacketType.AES_KEY_RESPONSE);
            packet.Write(loginClient.Id.ToByteArray());
            packet.Write(loginClient.CryptoManager.Key);
            packet.Write(loginClient.CryptoManager.IV);
            worldClient.SendPacket(packet, false);
        }
    }
}
