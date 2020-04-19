using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System;

namespace Imgeneus.Network.Packets.InternalServer
{
    public struct AesKeyRequestPacket : IDeserializedPacket
    {
        public Guid Guid { get; }

        public AesKeyRequestPacket(IPacketStream packet)
        {
            byte[] guidBytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                guidBytes[i] = packet.Read<byte>();
            }

            Guid = new Guid(guidBytes);
        }
    }
}
