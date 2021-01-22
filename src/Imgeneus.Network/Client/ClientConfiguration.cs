namespace Imgeneus.Network.Client
{
    public class ClientConfiguration
    {
        private const int defaultBufferSize = 2048;
        private const int defaultConnectionTimeout = 5000;
        private const ClientRetryOptions defaultRetryOptions = ClientRetryOptions.Onetime;

        /// <summary>
        /// Gets the <see cref="Client"/> host to connect.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the <see cref="Client"/> port to connect.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets the <see cref="Client"/> data buffer size to allocate during construction.
        /// </summary>
        public int BufferSize { get; }

        /// <summary>
        /// Gets the connection timeout in milliseconds.
        /// </summary>
        public int ConnectionTimeout { get; }

        /// <summary>
        /// Gets the <see cref="Client"/> retry options then tries to connect to the host.
        /// </summary>
        public ClientRetryOptions RetryOptions { get; }

        /// <summary>
        /// Gets the maximum number of retry attempts.
        /// </summary>
        public int MaxAttempts { get; }

        /// <summary>
        /// Creates a new <see cref="ClientConfiguration"/> instance.
        /// </summary>
        /// <param name="host">The host to connect.</param>
        /// <param name="port">The host port to connect.</param>
        /// <param name="bufferSize">The client buffer size.</param>
        /// <param name="connectionTimeout">The connection timeout in milliseconds.</param>
        /// <param name="retryOptions">The retry options then tries to connect to the host.</param>
        public ClientConfiguration(string host, int port, int bufferSize, int connectionTimeout, ClientRetryOptions retryOptions)
        {
            this.Host = host;
            this.Port = port;
            this.BufferSize = bufferSize;
            this.ConnectionTimeout = connectionTimeout;
            this.RetryOptions = retryOptions;
        }

        /// <summary>
        /// Creates a new <see cref="ClientConfiguration"/> with host and port, using default values.
        /// </summary>
        /// <param name="host">The host to connect.</param>
        /// <param name="port">The host port to connect.</param>
        public ClientConfiguration(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            this.BufferSize = defaultBufferSize;
            this.ConnectionTimeout = defaultConnectionTimeout;
            this.RetryOptions = defaultRetryOptions;
        }

    }
}
