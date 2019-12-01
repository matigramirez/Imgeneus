using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Login.Packets;
using Imgeneus.Network;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Packets.Login;
using System;
using System.Linq;

namespace Imgeneus.Login
{
    public static class LoginHandler
    {
        [PacketHandler(PacketType.LOGIN_HANDSHAKE)]
        public static void OnLoginHandshake(LoginClient client, IPacketStream packet)
        {
            throw new NotImplementedException();
        }

        [PacketHandler(PacketType.LOGIN_REQUEST)]
        public static void OnLoginRequest(LoginClient client, IPacketStream packet)
        {
            if (packet.Length != 52)
            {
                return;
            }

            var authenticationPacket = new AuthenticationPacket(packet);

            var result = Authentication(authenticationPacket.Username, authenticationPacket.Password);

            if (result != AuthenticationResult.SUCCESS)
            {
                LoginPacketFactory.AuthenticationFailed(client, result);
                return;
            }
            var loginServer = DependencyContainer.Instance.Resolve<ILoginServer>();

            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            DbUser dbUser = database.Users.SingleOrDefault(x => x.Username == authenticationPacket.Username);

            if (loginServer.IsClientConnected(dbUser.Id))
            {
                client.Disconnect();
                return;
            }

            dbUser.LastConnectionTime = DateTime.Now;
            database.Users.Update(dbUser);
            database.Complete();

            client.SetClientUserID(dbUser.Id);

            LoginPacketFactory.AuthenticationFailed(client, result, dbUser);
        }

        [PacketHandler(PacketType.SELECT_SERVER)]
        public static void OnSelectServer(LoginClient client, IPacketStream packet)
        { 
            if (packet.Length != 9)
            {
                return;
            }

            var selectServerPacket = new SelectServerPacket(packet);

            var server = DependencyContainer.Instance.Resolve<ILoginServer>();
            var worldInfo = server.GetWorldByID(selectServerPacket.WorldId);

            if (worldInfo == null)
            {
                LoginPacketFactory.SelectServerFailed(client, SelectServer.CannotConnect);
                return;
            }

            if (worldInfo.BuildVersion != selectServerPacket.BuildClient)
            {
                LoginPacketFactory.SelectServerFailed(client, SelectServer.VersionDoesntMatch);
                return;
            }

            if (worldInfo.ConnectedUsers >= worldInfo.MaxAllowedUsers)
            {
                LoginPacketFactory.SelectServerFailed(client, SelectServer.ServerSaturated);
                return;
            }

            LoginPacketFactory.SelectServerSuccess(client, worldInfo.Host);
        }

        [PacketHandler(PacketType.CLOSE_CONNECTION)]
        public static void OnCloseConnection(LoginClient client, IPacketStream packet)
        {
            throw new NotImplementedException();
            // TODO: Disconnect client from Internal-Server and World Server
        }

        public static AuthenticationResult Authentication(string username, string password)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();

            DbUser dbUser = database.Users.SingleOrDefault(x => x.Username == username);

            if (dbUser == null)
            {
                return AuthenticationResult.ACCOUNT_DONT_EXIST;
            }

            if (!dbUser.Password.Equals(password))
            {
                return AuthenticationResult.INVALID_PASSWORD;
            }

            return (AuthenticationResult)dbUser.Status;
        }
    }
}
