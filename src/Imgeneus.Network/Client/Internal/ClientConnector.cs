using System;
using System.Net.Sockets;
using System.Threading;

namespace Imgeneus.Network.Client.Internal
{
    internal sealed class ClientConnector : IDisposable
    {
        private readonly Client client;
        private readonly AutoResetEvent autoConnectEvent;

        /// <summary>
        /// Creates a new <see cref="ClientConnector"/> instance.
        /// </summary>
        /// <param name="client">The client to using the connector.</param>
        public ClientConnector(Client client)
        {
            this.client = client;
            this.autoConnectEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Connects to the remote host.
        /// </summary>
        /// <param name="connectSocketEvent">The connect event.</param>
        /// <returns>The socket error code.</returns>
        public SocketError Connect(SocketAsyncEventArgs connectSocketEvent)
        {
            SocketError error = SocketError.NotConnected;

            switch (this.client.ClientConfiguration.RetryOptions)
            {
                case ClientRetryOptions.Onetime:
                    error = this.InternalConnect(connectSocketEvent);
                    break;
                case ClientRetryOptions.Limited:
                    for (int i = 0; i < this.client.ClientConfiguration.MaxAttempts && !this.client.IsConnected; i++)
                    {
                        error = this.InternalConnect(connectSocketEvent);
                    }
                    break;
                case ClientRetryOptions.Infinite:
                    while (!this.client.IsConnected)
                    {
                        error = this.InternalConnect(connectSocketEvent);
                    }
                    break;
                default:
                    break;
            }

            return error;
        }

        /// <summary>
        /// Connects to the remote host.
        /// </summary>
        /// <param name="connectSocketEvent">The connect event.</param>
        /// <returns>The socket error code.</returns>
        private SocketError InternalConnect(SocketAsyncEventArgs connectSocketEvent)
        {
            this.client.Socket.ConnectAsync(connectSocketEvent);
            this.autoConnectEvent.WaitOne(this.client.ClientConfiguration.ConnectionTimeout);

            return connectSocketEvent.SocketError;
        }

        /// <summary>
        /// Release locks on connector.
        /// </summary>
        public void ReleaseConnectorLock() => this.autoConnectEvent.Set();

        public void Dispose()
        {
            this.autoConnectEvent.Dispose();
        }
    }
}
