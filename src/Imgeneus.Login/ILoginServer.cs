using Imgeneus.Network.Server;
using InterServer.Client;
using System.Collections.Generic;

namespace Imgeneus.Login
{
    public interface ILoginServer : IServer
    {
        /// <summary>
        /// Gets a list of all connected worlds.
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorldServerInfo> GetConnectedWorlds();

        /// <summary>
        /// Gets a world by id.
        /// </summary>
        /// <returns></returns>
        WorldServerInfo GetWorldByID(byte id);

        /// <summary>
        /// Gets a connected client by his username.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>The connected client.</returns>
        LoginClient GetClientByUserID(int userID);

        /// <summary>
        /// Verify if a client is connected to the login server.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <returns>True if client is connected.</returns>
        bool IsClientConnected(int userID);
    }
}
