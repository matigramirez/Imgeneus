using Imgeneus.Database.Entities;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Login;
using System.Linq;

namespace Imgeneus.Login.Packets
{
    internal static class LoginPacketFactory
    {
        public static void SendLoginHandshake(LoginClient client)
        {
            using Packet packet = new Packet(PacketType.LOGIN_HANDSHAKE);

            packet.Write<byte>(0);
            packet.Write<byte>((byte)client.CryptoManager.RSAPublicExponent.Length); // Exponent length
            packet.Write((byte)client.CryptoManager.RSAModulus.Length);
            packet.WritePaddedBytes(client.CryptoManager.RSAPublicExponent, 64);
            packet.WritePaddedBytes(client.CryptoManager.RSAModulus, 128);

            client.SendPacket(packet, false);
        }

        public static void AuthenticationFailed(LoginClient client, AuthenticationResult result)
        {
            using var packet = new Packet(PacketType.LOGIN_REQUEST);

            packet.Write<byte>((byte)result);
            packet.Write<byte[]>(new byte[21]);

            client.SendPacket(packet);
        }

        public static void SendServerList(LoginClient client)
        {
            using var packet = new Packet(PacketType.SERVER_LIST);

            var worlds = (client.Server as LoginServer).GetConnectedWorlds();

            packet.Write<byte>((byte)worlds.Count());

            foreach (var world in worlds)
            {
                packet.Write<byte>((byte)world.Id);
                packet.Write<byte>((byte)world.WorldStatus);
                packet.Write<ushort>(world.ConnectedUsers);
                packet.Write<ushort>(world.MaxAllowedUsers);
                packet.WriteString(world.Name, 32);
            }

            client.SendPacket(packet);
        }

        public static void AuthenticationSuccess(LoginClient client, AuthenticationResult result, DbUser user)
        {
            using var packet = new Packet(PacketType.LOGIN_REQUEST);

            packet.Write<byte>((byte)result);
            packet.Write<int>(user.Id);
            packet.Write<byte>(user.Authority);
            packet.Write<byte[]>(client.Id.ToByteArray());

            client.SendPacket(packet);
            SendServerList(client);
        }

        public static void SelectServerFailed(LoginClient client, SelectServer error)
        {
            using var packet = new Packet(PacketType.SELECT_SERVER);
            packet.Write<sbyte>((sbyte)error);
            packet.Write(new byte[4]);

            client.SendPacket(packet);
        }

        public static void SelectServerSuccess(LoginClient client, byte[] worldIp)
        {
            using var packet = new Packet(PacketType.SELECT_SERVER);
            packet.Write<sbyte>((sbyte)SelectServer.Success);
            packet.Write(worldIp);

            client.SendPacket(packet);
        }
    }
}
