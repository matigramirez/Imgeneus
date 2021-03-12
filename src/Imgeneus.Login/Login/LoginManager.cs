using System;
using System.Linq;
using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Login.Packets;
using Imgeneus.Network.Packets.Game;
using Imgeneus.Network.Packets.Login;
using Imgeneus.Network.Server;

namespace Imgeneus.Login
{
    /// <summary>
    /// Login manager is responsible for handling login packets.
    /// These are packets like login, select server etc.
    /// </summary>
    public class LoginManager : IDisposable
    {
        private readonly LoginClient _client;
        private readonly ILoginServer _server;
        private readonly IDatabase _database;

        public LoginManager(LoginClient client, ILoginServer server, IDatabase database)
        {
            _client = client;
            _server = server;
            _database = database;
            _client.OnPacketArrived += Client_OnPacketArrived;
        }

        public void Dispose()
        {
            _client.OnPacketArrived -= Client_OnPacketArrived;
            _database.Dispose();
        }

        private void Client_OnPacketArrived(ServerClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case AuthenticationPacket authenticationPacket:
                    HandleLoginRequest(authenticationPacket);
                    break;
                case OAuthAuthenticationPacket oauthPacket:
                    HandleOauthAuthentication(oauthPacket);
                    break;
                case SelectServerPacket selectServerPacket:
                    HandleSelectServer(selectServerPacket);
                    break;
            }
        }

        private void HandleLoginRequest(AuthenticationPacket authenticationPacket)
        {
            HandleAuthentication(authenticationPacket.Username, authenticationPacket.Password);
        }

        private void HandleOauthAuthentication(OAuthAuthenticationPacket packet)
        {
            // TODO(OAuth): Should we actually implement OAuth? Perhaps a custom updater distribution is desired.
            // For now, let's just parse the key as a username/password combination.
            var parts = packet.key.Split(":");
            var username = parts.FirstOrDefault();
            var password = parts.LastOrDefault();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                /*
                 * The client doesn't handle unsuccessful login responses after attempting to login with an OAuth key,
                 * as it expects the key to always be valid (having been authenticated in a prior step via the updater).
                 */
                _client.Disconnect();
                return;
            }
            
            HandleAuthentication(username, password);
        }

        private void HandleAuthentication(string username, string password)
        {
            var result = Authentication(username, password);
            if (result != AuthenticationResult.SUCCESS)
            {
                LoginPacketFactory.AuthenticationFailed(_client, result);
                return;
            }

            DbUser dbUser = _database.Users.First(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (_server.IsClientConnected(dbUser.Id))
            {
                _client.Disconnect();
                return;
            }

            dbUser.LastConnectionTime = DateTime.UtcNow;
            _database.Users.Update(dbUser);
            _database.SaveChanges();

            _client.SetClientUserID(dbUser.Id);

            LoginPacketFactory.AuthenticationSuccess(_client, result, dbUser);
        }

        private void HandleSelectServer(SelectServerPacket selectServerPacket)
        {
            var worldInfo = _server.GetWorldByID(selectServerPacket.WorldId);

            if (worldInfo == null)
            {
                LoginPacketFactory.SelectServerFailed(_client, SelectServer.CannotConnect);
                return;
            }

            // For some reason, the current game.exe has version -1. Maybe this is somehow connected with decrypted packages?
            // In any case, for now client version check is disabled.
            if (false && worldInfo.BuildVersion != selectServerPacket.BuildClient && selectServerPacket.BuildClient != -1)
            {
                LoginPacketFactory.SelectServerFailed(_client, SelectServer.VersionDoesntMatch);
                return;
            }

            if (worldInfo.ConnectedUsers >= worldInfo.MaxAllowedUsers)
            {
                LoginPacketFactory.SelectServerFailed(_client, SelectServer.ServerSaturated);
                return;
            }

            LoginPacketFactory.SelectServerSuccess(_client, worldInfo.Host);
        }

        public AuthenticationResult Authentication(string username, string password)
        {
            DbUser dbUser = _database.Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (dbUser == null)
            {
                return AuthenticationResult.ACCOUNT_DONT_EXIST;
            }

            if (dbUser.IsDeleted)
            {
                return AuthenticationResult.ACCOUNT_IN_DELETE_PROCESS_1;
            }

            if (!dbUser.Password.Equals(password))
            {
                return AuthenticationResult.INVALID_PASSWORD;
            }

            return (AuthenticationResult)dbUser.Status;
        }
    }
}
