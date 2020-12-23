using Imgeneus.Network.Common;
using Imgeneus.Network.Server.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Imgeneus.Network.Server
{
    /// <summary>
    /// Provides a basic mechanism to start a TCP server.
    /// </summary>
    public class Server<T> : Connection, IServer where T : class, IServerClient
    {
        protected readonly ConcurrentDictionary<Guid, T> clients;
        private readonly BufferManager bufferManager;
        private readonly ServerReceiver receiver;
        private readonly ServerSender sender;
        private readonly ServerAcceptor<T> acceptor;
        private readonly ILogger _logger;

        /// <inheritdoc />
        public event EventHandler Started;

        /// <inheritdoc />
        public event EventHandler Stopped;

        /// <inheritdoc />
        public event EventHandler<Exception> Error;

        /// <inheritdoc />
        public event EventHandler<IServerClient> ClientConnected;

        /// <inheritdoc />
        public event EventHandler<IServerClient> ClientDisconnected;

        /// <inheritdoc />
        public ServerConfiguration ServerConfiguration { get; }

        /// <inheritdoc />
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Server{T}"/> using <see cref="Server.ServerConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="Server.ServerConfiguration"/>.</param>
        public Server(ServerConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            this.ServerConfiguration = configuration ?? throw new ArgumentNullException("Configuration can't be null.");
            this.clients = new ConcurrentDictionary<Guid, T>();
            this.acceptor = new ServerAcceptor<T>(this);
            this.receiver = new ServerReceiver(this);
            this.sender = new ServerSender();
            this.bufferManager = new BufferManager(configuration.MaximumNumberOfConnections, configuration.ClientBufferSize);
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public virtual void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            this.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.Socket.Bind(NetworkHelper.CreateIPEndPoint(this.ServerConfiguration.Host, this.ServerConfiguration.Port));
            this.Socket.Listen(this.ServerConfiguration.Backlog);
            this.FillPool();

            this.IsRunning = true;
            this.OnStart();
            this.sender.Start();
            this.acceptor.StartAccept();

        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException("Server is not running.");
            }

            this.sender.Stop();
            this.Socket.Close();
            this.OnStop();
        }

        /// <inheritdoc />
        public IServerClient GetClient(Guid clientId)
            => this.clients.TryGetValue(clientId, out T client) ? client : default;

        /// <inheritdoc />
        public void DisconnectClient(IServerClient client) => this.DisconnectClient(client.Id);

        /// <inheritdoc />
        public void DisconnectClient(Guid clientId)
        {
            if (this.clients.TryRemove(clientId, out T client))
            {
                this.OnClientDisconnected(client);
                client.Dispose();
            }
        }

        /// <inheritdoc />
        public void SendPacketTo(IServerClient client, byte[] packetData)
            => this.sender.AddPacketToQueue(new PacketData(client, packetData));

        /// <inheritdoc />
        public void SendPacketTo(IEnumerable<IServerClient> clients, byte[] packetData)
        {
            foreach (var client in clients)
            {
                this.SendPacketTo(client, packetData);
            }
        }

        /// <inheritdoc />
        public void SendPacketToAll(byte[] packetData)
            => this.SendPacketTo(this.clients.Values, packetData);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            this.receiver.Dispose();
            this.sender.Dispose();
            base.Dispose(disposing);
        }

        protected virtual void OnStart()
        {
            this.Started?.Invoke(this, null);
        }

        protected virtual void OnStop()
        {
            this.Stopped?.Invoke(this, null);
        }

        protected virtual void OnError(Exception exception)
        {
            this.Error?.Invoke(this, exception);
        }

        protected virtual void OnClientConnected(T client)
        {
            this.ClientConnected?.Invoke(this, client);
        }

        protected virtual void OnClientDisconnected(T client)
        {
            this.ClientDisconnected?.Invoke(this, client);
        }

        private void FillPool()
        {
            for (int i = 0; i < this.ServerConfiguration.MaximumNumberOfConnections; i++)
            {
                var readSocket = new SocketAsyncEventArgs();
                var writeSocket = new SocketAsyncEventArgs();

                readSocket.Completed += this.OnSocketCompleted;
                writeSocket.Completed += this.OnSocketCompleted;

                this.bufferManager.SetBuffer(readSocket);
                this.bufferManager.SetBuffer(writeSocket);

                this.receiver.ReadPool.Push(readSocket);
                this.sender.WritePool.Push(writeSocket);
            }
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
                    case SocketAsyncOperation.Accept:
                        this.acceptor.ProcessAccept(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        this.receiver.Receive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        this.sender.SendOperationCompleted(e);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected SocketAsyncOperation.");
                }
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }
        }

        /// <summary>
        /// Creates a new accepted client instance.
        /// </summary>
        /// <param name="acceptedSocketEvent"></param>
        internal void CreateClient(SocketAsyncEventArgs acceptedSocketEvent)
        {
            if (this.receiver.ReadPool.TryPop(out SocketAsyncEventArgs readSocket))
            {
                var client = Activator.CreateInstance(typeof(T), this, acceptedSocketEvent.AcceptSocket, _logger) as T;

                if (this.clients.TryAdd(client.Id, client))
                {
                    this.OnClientConnected(client);

                    readSocket.UserToken = client;

                    if (!acceptedSocketEvent.AcceptSocket.ReceiveAsync(readSocket))
                    {
                        this.receiver.Receive(readSocket);
                    }
                }
                else
                {
                    throw new InvalidProgramException($"Client {client.Id} can't add to client list.");
                }
            }
        }
    }
}
