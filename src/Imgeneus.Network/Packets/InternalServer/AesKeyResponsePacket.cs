using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System;

namespace Imgeneus.Network.Packets.InternalServer
{
    public struct AesKeyResponsePacket : IDeserializedPacket
    {
        public Guid Guid { get; }

        public byte[] Key { get; }

        public byte[] IV { get; }

        public AesKeyResponsePacket(IPacketStream packet)
        {
            // First goes session id.
            byte[] guidBytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                guidBytes[i] = packet.Read<byte>();
            }

            Guid = new Guid(guidBytes);

            // After that aes key.
            Key = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                Key[i] = packet.Read<byte>();
            }

            // After that iv.
            IV = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                IV[i] = packet.Read<byte>();
            }
        }
    }
}
