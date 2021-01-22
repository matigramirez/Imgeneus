namespace Imgeneus.Network.Server
{
    /// <summary>
    /// Provides properties to configure a new <see cref="Server"/>.
    /// </summary>
    public class ServerConfiguration
    {
        private const int defaultBacklog = 50;
        private const int defaultMaxNumberOfConnections = 1000;
        private const int defaultClientBufferSize = 2048;

        /// <summary>
        /// Gets the server's listening host.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the server's listening port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the maximum of pending connections queue.
        /// </summary>
        public int Backlog { get; }

        /// <summary>
        /// Gets the maximum number of connections allowed on the server.
        /// </summary>
        public int MaximumNumberOfConnections { get; }

        /// <summary>
        /// Gets the handled client buffer size.
        /// </summary>
        public int ClientBufferSize { get; }

        /// <summary>
        /// Creates a new <see cref="ServerConfiguration"/> instance.
        /// </summary>
        /// <param name="host">The server host ip address.</param>
        /// <param name="port">The server listening port.</param>
        public ServerConfiguration(string host, int port, int maxNumberOfConnections)
        {
            this.Host = host;
            this.Port = port;
            this.Backlog = defaultBacklog;
            this.MaximumNumberOfConnections = maxNumberOfConnections;
            this.ClientBufferSize = defaultClientBufferSize;
        }

        /// <summary>
        /// Creates a new <see cref="ServerConfiguration"/> instance.
        /// </summary>
        /// <param name="host">The server host ip address.</param>
        /// <param name="port">The server listening port.</param>
        public ServerConfiguration(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            this.Backlog = defaultBacklog;
            this.MaximumNumberOfConnections = defaultMaxNumberOfConnections;
            this.ClientBufferSize = defaultClientBufferSize;
        }

        /// <summary>
        /// Creates a new <see cref="ServerConfiguration"/> instance.
        /// </summary>
        /// <param name="host">The server host ip address.</param>
        /// <param name="port">The server listening port.</param>
        /// <param name="backlog">The maximum of pending connections.</param>
        /// <param name="maximumNumberOfConnections">The maximum number of connections allowed.</param>
        /// <param name="clientBufferSize">The handle client buffer size.</param>
        public ServerConfiguration(string host, int port, int backlog, int maximumNumberOfConnections, int clientBufferSize)
        {
            this.Host = host;
            this.Port = port;
            this.Backlog = backlog;
            this.MaximumNumberOfConnections = maximumNumberOfConnections;
            this.ClientBufferSize = clientBufferSize;
        }
    }
}
