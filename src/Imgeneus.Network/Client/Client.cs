using Imgeneus.Network.Client.Internal;
using Imgeneus.Network.Common;
using Imgeneus.Network.Data;
using System;
using System.Net.Sockets;

namespace Imgeneus.Network.Client
{
    /// <summary>
    /// Provides a mechanism for creating managed TCP clients.
    /// </summary>
    public abstract class Client : Connection, IClient
    {
        private readonly ClientConnector connector;
        private ClientReceiver receiver;
        private ClientSender sender;
        private SocketAsyncEventArgs connectSocketArgs;
        private SocketAsyncEventArgs sendSocketArgs;
        private SocketAsyncEventArgs receiveSocketArgs;

        /// <inheritdoc />
        public bool IsConnected => this.Socket.Connected;

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <inheritdoc />
        public ClientConfiguration ClientConfiguration { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="Client"/> instance.
        /// </summary>
        /// <param name="socketConnection"></param>
        protected Client(ClientConfiguration clientConfiguration)
        {
            this.ClientConfiguration = clientConfiguration;
            this.connector = new ClientConnector(this);

            this.connectSocketArgs = CreateSocketEventArgs(null);
            this.connectSocketArgs.RemoteEndPoint = NetworkHelper.CreateIPEndPoint(this.ClientConfiguration.Host, this.ClientConfiguration.Port);


            this.sendSocketArgs = CreateSocketEventArgs(null);

            this.receiveSocketArgs = CreateSocketEventArgs(1024);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Client is already running.");
            }

            if (this.IsConnected)
            {
                throw new InvalidOperationException("Client is already connected to remote host.");
            }

            if (this.ClientConfiguration == null)
            {
                throw new ArgumentNullException(nameof(this.ClientConfiguration), "Undefined Client configuration.");
            }

            if (this.ClientConfiguration.Port <= 0)
            {
                throw new ArgumentException($"Invalid port number '{this.ClientConfiguration.Port}' in configuration.", nameof(this.ClientConfiguration.Port));

            }

            if (NetworkHelper.BuildIPAddress(this.ClientConfiguration.Host) == null)
            {
                throw new ArgumentException($"Invalid host address '{this.ClientConfiguration.Host}' in configuration", nameof(this.ClientConfiguration.Host));
            }


            if (this.ClientConfiguration.BufferSize <= 0)
            {
                throw new ArgumentException($"Invalid buffer size '{this.ClientConfiguration.BufferSize}' in configuration.", nameof(this.ClientConfiguration.BufferSize));
            }

            SocketError error = this.connector.Connect(this.connectSocketArgs);

            if (!this.IsConnected && error != SocketError.Success)
            {
                this.OnError(new InvalidOperationException("No se puede conectar con el servidor."));
                return;
            }

            this.sender = new ClientSender(sendSocketArgs);
            this.receiver = new ClientReceiver(this, receiveSocketArgs);

            this.IsRunning = true;
            this.sender.Start();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!IsRunning)
            {
                return;
            }
            this.IsRunning = false;
            this.Socket.Disconnect(true);
            this.sender.Stop();
            this.connectSocketArgs.Dispose();
        }

        /// <inheritdoc />
        public abstract void HandlePacket(IPacketStream packet);

        /// <inheritdoc />
        public void SendPacket(IPacketStream packet) => this.sender.AddPacketToQueue(new PacketData(this, packet.Buffer));
        /// <summary>
        /// Triggered when the client is connected to the remote end point.
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// Triggered when the client is disconnected from the remote end point.
        /// </summary>
        protected abstract void OnDisconnected();

        /// <summary>
        /// Triggered when a error on the socket happend
        /// </summary>
        /// <param name="socketError"></param>
        protected abstract void OnError(Exception exception);

        /// <summary>
        /// Creates a new <see cref="SocketAsyncEventArgs"/> for a <see cref="Client"/>.
        /// </summary>
        /// <param name="bufferSize">Buffer size</param>
        /// <returns></returns>
        private SocketAsyncEventArgs CreateSocketEventArgs(int? bufferSize)
        {
            var socketEvent = new SocketAsyncEventArgs
            {
                UserToken = this
            };

            socketEvent.Completed += this.OnSocketCompleted;
            if (bufferSize.HasValue)
            {
                socketEvent.SetBuffer(new byte[bufferSize.Value], 0, bufferSize.Value);
            }

            return socketEvent;
        }

        /// <summary>
        /// Method called when a <see cref="SocketAsyncEventArgs"/> completes an async operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnSocketCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        this.OnConnected();
                        this.connector.ReleaseConnectorLock();
                        break;
                    case SocketAsyncOperation.Receive:
                        this.receiver.Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        this.sender.SendOperationCompleted(e);
                        break;
                    default: throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
                }
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            this.sender.Dispose();
            this.connector.Dispose();
            base.Dispose(disposing);
            this.connectSocketArgs.Dispose();
            this.sendSocketArgs.Dispose();
            this.receiveSocketArgs.Dispose();
        }
    }
}
