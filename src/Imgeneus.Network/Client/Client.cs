using Imgeneus.Network.Client.Internal;
using Imgeneus.Network.Common;
using Imgeneus.Network.Data;
using System;
using System.Net.Sockets;

namespace Imgeneus.Network.Client
{
    /// <summary>
    /// Provides a mechanism for creating managed TCP clients.
    /// TODO: maybe I need to rethink this implementation later, it's a little messy.
    /// On the other hand, this client is only used for inter server communication (game server <=> login server).
    /// Maybe I'll rethink this implementation during encryption implementation between game and login servers.
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
        public bool IsConnected => Socket.Connected;

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
            ClientConfiguration = clientConfiguration;
            connector = new ClientConnector(this);

            connectSocketArgs = CreateSocketEventArgs(null);
            connectSocketArgs.RemoteEndPoint = NetworkHelper.CreateIPEndPoint(ClientConfiguration.Host, ClientConfiguration.Port);


            sendSocketArgs = CreateSocketEventArgs(null);
            sendSocketArgs.RemoteEndPoint = NetworkHelper.CreateIPEndPoint(ClientConfiguration.Host, ClientConfiguration.Port);

            receiveSocketArgs = CreateSocketEventArgs(1024);
            receiveSocketArgs.RemoteEndPoint = NetworkHelper.CreateIPEndPoint(ClientConfiguration.Host, ClientConfiguration.Port);
        }

        /// <inheritdoc />
        public void Connect()
        {
            if (IsRunning)
                throw new InvalidOperationException("Client is already running.");

            if (IsConnected)
                throw new InvalidOperationException("Client is already connected to remote host.");

            if (ClientConfiguration == null)
                throw new ArgumentNullException(nameof(ClientConfiguration), "Undefined Client configuration.");

            if (ClientConfiguration.Port <= 0)
                throw new ArgumentException($"Invalid port number '{ClientConfiguration.Port}' in configuration.", nameof(ClientConfiguration.Port));

            if (NetworkHelper.BuildIPAddress(ClientConfiguration.Host) == null)
                throw new ArgumentException($"Invalid host address '{ClientConfiguration.Host}' in configuration", nameof(ClientConfiguration.Host));

            SocketError error = connector.Connect(connectSocketArgs);

            if (!IsConnected && error != SocketError.Success)
            {
                OnError(new InvalidOperationException("Could not connect to login server."));
                return;
            }

            receiver = new ClientReceiver(this);
            if (!Socket.ReceiveAsync(receiveSocketArgs))
            {
                receiver.Receive(receiveSocketArgs);
            }

            IsRunning = true;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (!IsRunning)
            {
                return;
            }
            IsRunning = false;
            Socket.Disconnect(true);
            sender.Stop();
            connectSocketArgs.Dispose();
        }

        /// <inheritdoc />
        public abstract void HandlePacket(IPacketStream packet);

        /// <inheritdoc />
        public void SendPacket(IPacketStream packet) => sender.AddPacketToQueue(new PacketData(this, packet.Buffer));

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

            socketEvent.Completed += OnSocketCompleted;
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
        internal void OnSocketCompleted(object s, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Connect:
                        sender = new ClientSender(sendSocketArgs);
                        sender.Start();
                        OnConnected();
                        connector.ReleaseConnectorLock();
                        break;
                    case SocketAsyncOperation.Receive:
                        receiver.Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        sender.SendOperationCompleted(e);
                        break;
                    default: throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
                }
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            sender.Dispose();
            connector.Dispose();
            base.Dispose(disposing);
            connectSocketArgs.Dispose();
            sendSocketArgs.Dispose();
            receiveSocketArgs.Dispose();
        }
    }
}
