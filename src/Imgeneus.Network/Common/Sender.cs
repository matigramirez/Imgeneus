using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.Network.Common
{
    internal abstract class Sender : IDisposable
    {
        private bool disposedValue;
        private readonly BlockingCollection<PacketData> sendingQueue;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;
        protected readonly AutoResetEvent autoSendEvent;

        /// <summary>
        /// Creates a new <see cref="Sender"/> instance.
        /// </summary>
        protected Sender()
        {
            this.sendingQueue = new BlockingCollection<PacketData>();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.autoSendEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Starts the sender task.
        /// </summary>
        public void Start() => Task.Factory.StartNew(this.ProcessSendingQueue, this.cancellationToken);

        /// <summary>
        /// Stops the sender task.
        /// </summary>
        public void Stop() => this.cancellationTokenSource.Cancel(false);

        /// <summary>
        /// Adds a new packet to the sender queue.
        /// </summary>
        /// <param name="packet"></param>
        public void AddPacketToQueue(PacketData packet) => this.sendingQueue.Add(packet);

        /// <summary>
        /// Process sending queue.
        /// </summary>
        private void ProcessSendingQueue()
        {
            while (true)
            {
                try
                {
                    PacketData packet = this.sendingQueue.Take(this.cancellationToken);

                    if (packet.Connection != null && packet.Data != null)
                    {
                        this.SendPacket(packet);
                    }
                }
                catch
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sends a packet to a connection.
        /// </summary>
        /// <param name="packetData"></param>
        public abstract void SendPacket(PacketData packetData);

        /// <summary>
        /// Completes a send operation.
        /// </summary>
        /// <param name="e">Socket that finished the operation.</param>
        public abstract void SendOperationCompleted(SocketAsyncEventArgs e);

        /// <summary>
        /// Dispose the <see cref="Sender"/> internal resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Stop();
                    this.sendingQueue.Dispose();
                    this.cancellationTokenSource.Dispose();
                    this.autoSendEvent.Dispose();
                }

                this.disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose() => this.Dispose(true);
    }
}
