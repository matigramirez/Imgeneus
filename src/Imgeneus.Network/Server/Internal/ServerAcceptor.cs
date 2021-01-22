using System.Net.Sockets;

namespace Imgeneus.Network.Server.Internal
{
    internal sealed class ServerAcceptor<T> where T : class, IServerClient
    {
        private readonly Server<T> server;

        /// <summary>
        /// Creates a new <see cref="ServerAcceptor{T}"/> instance.
        /// </summary>
        /// <param name="server"></param>
        public ServerAcceptor(Server<T> server)
        {
            this.server = server;
        }

        /// <summary>
        /// Start accepting clients.
        /// </summary>
        public void StartAccept()
        {
            var socketAcceptorAsync = new SocketAsyncEventArgs();
            socketAcceptorAsync.Completed += (sender, e) => this.server.OnSocketCompleted(sender, e);

            // post accepts on the listening socket
            this.Accept(socketAcceptorAsync);
        }

        /// <summary>
        /// The accept operation on the server's listening socket.
        /// </summary>
        /// <param name="args">The <see cref="SocketAsyncEventArgs"/> of the client.</param>
        private void Accept(SocketAsyncEventArgs e)
        {
            e.AcceptSocket = null;

            if (!this.server.Socket.AcceptAsync(e))
            {
                this.ProcessAccept(e);
            }
        }

        /// <summary>
        /// Process the accept connection async operation
        /// </summary>
        /// <param name="args"></param>
        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                this.server.CreateClient(e);

                // Accept the next connection request
                this.Accept(e);
            }
        }
    }
}
