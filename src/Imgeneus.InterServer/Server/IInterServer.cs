using InterServer.Client;
using InterServer.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace InterServer.Server
{
    /// <summary>
    /// Server for communication between login and world server.
    /// </summary>
    public interface IInterServer
    {
        /// <summary>
        /// Gets the list of the connected worlds.
        /// </summary>
        public IList<WorldServerInfo> WorldServers { get; }

        /// <summary>
        /// Adds world to known servers.
        /// </summary>
        public void AddWorldServer(WorldServerInfo worldInfo);

        /// <summary>
        /// Collection of client sessions.
        /// </summary>
        public ConcurrentDictionary<Guid, KeyPair> Sessions { get; }
    }
}
