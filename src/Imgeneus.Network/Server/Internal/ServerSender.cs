using Imgeneus.Network.Common;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Imgeneus.Network.Server.Internal
{
    internal sealed class ServerSender : Sender, IDisposable
    {

        /// <summary>
        /// Gets the sender <see cref="SocketAsyncEventArgs"/> pool.
        /// </summary>
        public ConcurrentStack<SocketAsyncEventArgs> WritePool { get; }

        /// <summary>
        /// Creates a new <see cref="ServerSender"/> instace.
        /// </summary>
        public ServerSender()
        {
            this.WritePool = new ConcurrentStack<SocketAsyncEventArgs>();
        }

        /// <inheritdoc />
        public override void SendPacket(PacketData packetData)
        {
            if (this.WritePool.TryPop(out SocketAsyncEventArgs writeSocket))
            {
                writeSocket.SetBuffer(packetData.Data, 0, packetData.Data.Length);
                writeSocket.UserToken = packetData.Connection;

                if (!packetData.Connection.Socket.SendAsync(writeSocket))
                {
                    this.SendOperationCompleted(writeSocket);
                }
            }
            else
            {
                this.autoSendEvent.WaitOne();
                this.SendPacket(packetData);
            }
        }

        /// <inheritdoc />
        public override void SendOperationCompleted(SocketAsyncEventArgs e)
        {
            this.WritePool.Push(e);
            this.autoSendEvent.Set();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            foreach (var socket in this.WritePool)
            {
                socket.Dispose();
            }

            this.WritePool.Clear();
            base.Dispose(disposing);
        }
    }
}
