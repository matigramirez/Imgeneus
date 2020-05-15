using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;

namespace Imgeneus.Network.Server.Internal
{
    internal sealed class ServerReceiver : IDisposable
    {
        private bool disposedValue;
        private readonly IServer server;

        /// <summary>
        /// Gets the receive <see cref="SocketAsyncEventArgs"/> pool
        /// </summary>
        public ConcurrentStack<SocketAsyncEventArgs> ReadPool { get; }

        /// <summary>
        /// Creates a new <see cref="ServerReceiver"/> instance.
        /// </summary>
        /// <param name="server">The parent server.</param>
        public ServerReceiver(IServer server)
        {
            this.server = server;
            this.ReadPool = new ConcurrentStack<SocketAsyncEventArgs>();
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
                if (!(e.UserToken is ServerClient client))
                {
                    return;
                }


                var receivedBuffer = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, receivedBuffer, 0, e.BytesTransferred);

                if (receivedBuffer.Length == BitConverter.ToUInt16(new byte[] { receivedBuffer[0], receivedBuffer[1] }))
                {
                    DispatchPacket(client, receivedBuffer);
                }
                else
                {
                    // Case when packets pasted together.
                    var index = 0;
                    while (index != receivedBuffer.Length)
                    {
                        var length = BitConverter.ToUInt16(new byte[] { receivedBuffer[index], receivedBuffer[index + 1] });
                        var tempBuffer = new byte[length];
                        Array.Copy(receivedBuffer, index, tempBuffer, 0, length);
                        DispatchPacket(client, tempBuffer);

                        index += length;
                    }
                }

                if (!client.Socket.ReceiveAsync(e))
                {
                    this.Receive(e);
                }
            }
            else
            {
                this.CloseConnection(e);
            }
        }

        /// <summary>
        /// Closes the current socket event connection.
        /// </summary>
        /// <param name="e"></param>
        private void CloseConnection(SocketAsyncEventArgs e)
        {
            Array.Clear(e.Buffer, 0, e.Buffer.Length);
            this.ReadPool.Push(e);

            if (e.UserToken is ServerClient client)
            {
                this.server.DisconnectClient(client.Id);
            }
        }

        /// <summary>
        /// Dispatch an incoming packets to a client.
        /// </summary>
        /// <param name="client">the client.</param>
        /// <param name="packetData">Received packet data.</param>
        private void DispatchPacket(ServerClient client, byte[] packetData)
        {
            using IPacketStream packet = new PacketStream(packetData);
            if (packet.PacketType != PacketType.LOGIN_HANDSHAKE && packet.PacketType != PacketType.GAME_HANDSHAKE &&
                // TODO: this internal packets should be also encrypted somehow.
                packet.PacketType != PacketType.AUTH_SERVER && packet.PacketType != PacketType.AES_KEY_REQUEST)
            {
                byte[] encryptedBytes = packetData.Skip(2).ToArray();
                byte[] decrypted = client.CryptoManager.Decrypt(encryptedBytes);

                var finalDecrypted = new byte[packetData.Length];
                finalDecrypted[0] = packetData[0];
                finalDecrypted[1] = packetData[1];

                for (var i = 0; i < decrypted.Length; i++)
                {
                    finalDecrypted[i + 2] = decrypted[i];
                }
                using IPacketStream decryptedPacket = new PacketStream(finalDecrypted);
                client.HandlePacket(decryptedPacket);
            }
            else // Handshake packets are not encrypted.
            {
                client.HandlePacket(packet);
            }
        }

        /// <summary>
        /// Disposes the <see cref="ServerReceiver"/> resources.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                foreach (var socket in this.ReadPool)
                {
                    socket.Dispose();
                }
                this.ReadPool.Clear();
                this.disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose() => this.Dispose(true);
    }
}
