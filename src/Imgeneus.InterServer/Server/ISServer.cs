using InterServer.Client;
using InterServer.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace InterServer.Server
{
    public class ISServer : IInterServer
    {
        private readonly ILogger<IInterServer> _logger;

        public ISServer(ILogger<IInterServer> log)
        {
            _logger = log;
        }

        /// <inheritdoc />
        public ConcurrentDictionary<Guid, KeyPair> Sessions { get; private set; } = new ConcurrentDictionary<Guid, KeyPair>();

        private readonly List<WorldServerInfo> _worlds = new List<WorldServerInfo>();

        /// <inheritdoc />
        public IList<WorldServerInfo> WorldServers => _worlds;

        public void AddWorldServer(WorldServerInfo info)
        {
            _worlds.Add(info);
            _logger.LogInformation($"New world server {info.Name} connected.");
        }
    }
}
