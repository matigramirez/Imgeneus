using Imgeneus.Network.Data;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Imgeneus.Network.Client.Internal
{
    internal sealed class ClientReceiver
    {
        private readonly IClient client;

        public ClientReceiver(IClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Receives incoming data.
        /// </summary>
        /// <param name="e">Current socket async event args.</param>
        public void Receive(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("Cannot receive data from a null socket event.", nameof(e));
            }

            if (e.SocketError == SocketError.Success && e.BytesTransferred >= 4)
            {

                var receivedBuffer = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, receivedBuffer, 0, e.BytesTransferred);

                using IPacketStream packet = new PacketStream(receivedBuffer);
                this.client.HandlePacket(packet);

                if (!this.client.Socket.ReceiveAsync(e))
                {
                    this.Receive(e);
                }
            }
        }
    }
}
