using System;
using System.Net.Sockets;

namespace Imgeneus.Network.Common
{
    public class Connection : IConnection
    {
        private bool disposedValue;

        public bool IsDispose => disposedValue;

        /// <summary>
        /// Gets the connection unique idenfified.
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Gets the connection socket.
        /// </summary>
        public Socket Socket { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="Connection"/>.
        /// </summary>
        /// <param name="socketConnection">The <see cref="System.Net.Sockets.Socket"/> connection./</param>
        protected Connection(Socket socketConnection)
        {
            this.Id = Guid.NewGuid();
            this.Socket = socketConnection;
        }
        /// <summary>
        /// Creates a new <see cref="Connection"/>.
        /// </summary>
        protected Connection()
        {
            this.Id = Guid.NewGuid();
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Dipose the <see cref="Connection"/> resources.
        /// </summary>
        /// <param name="disposing">The disposing value.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                this.Socket.Dispose();

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
