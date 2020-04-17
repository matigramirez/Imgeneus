using Imgeneus.Network.Common;
using Imgeneus.Network.Data;
using Imgeneus.Network.Packets;
using Imgeneus.Network.Server.Crypto;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using static Imgeneus.Network.Server.IServerClient;

namespace Imgeneus.Network.Server
{
    public abstract class ServerClient : Connection, IServerClient
    {
        /// <inheritdoc />
        public IServer Server { get; internal set; }

        /// <inheritdoc />
        public string RemoteEndPoint { get; }

        /// <summary>
        /// Crypto manager is responsible for the whole cryptography.
        /// </summary>
        public CryptoManager CryptoManager { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ServerClient"/> instance.
        /// </summary>
        /// <param name="acceptedSocket">Net User socket</param>
        protected ServerClient(IServer server, Socket acceptedSocket)
            : base(acceptedSocket)
        {
            Server = server;
            RemoteEndPoint = acceptedSocket.RemoteEndPoint.ToString();
            CryptoManager = new CryptoManager();
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            Dispose();
            Server.DisconnectClient(this.Id);
        }

        /// <inheritdoc />
        public abstract void HandlePacket(IPacketStream packet);

        /// <inheritdoc />
        public virtual void SendPacket(IPacketStream packet, bool shouldEncrypt = true)
        {
            byte[] bytes;

            if (shouldEncrypt)
            {
                bytes = EncryptPacket(packet);
            }
            else
            {
                bytes = packet.Buffer;
            }
            Server.SendPacketTo(this, bytes);
        }

        /// <inheritdoc />
        public abstract Dictionary<PacketType, PacketDeserializeHandler> PacketHandlers
        {
            get;
        }

        /// <summary>
        /// Perform bytes encryption before each send to client.
        /// </summary>
        /// <param name="incomingBytes">incomming bytes</param>
        /// <returns>encrypted bytes</returns>
        private byte[] EncryptPacket(IPacketStream packet)
        {
            var rawBytes = packet.Buffer;
            byte[] temp = new byte[rawBytes.Length - 2]; // Skip 2 bytes, because it's packet size and we should not encrypt them.
            Array.Copy(rawBytes, 2, temp, 0, rawBytes.Length - 2);

            // Calculated encrypted bytes.
            var encryptedBytes = CryptoManager.EncryptAES(temp);

            var resultBytes = new byte[rawBytes.Length];

            // Copy packet length.
            resultBytes[0] = rawBytes[0];
            resultBytes[1] = rawBytes[1];

            // Copy encrypted bytes.
            for (var i = 0; i < encryptedBytes.Length; i++)
            {
                resultBytes[i + 2] = encryptedBytes[i];
            }

            return resultBytes;
        }
    }
}
