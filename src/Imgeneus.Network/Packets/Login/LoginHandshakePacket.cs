using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System;
using System.Numerics;

namespace Imgeneus.Network.Packets.Login
{
    public struct LoginHandshakePacket : IDeserializedPacket
    {
        public BigInteger EncyptedNumber { get; }

        public LoginHandshakePacket(IPacketStream packet)
        {
            // NB! 129 is one byte more, than client sends. The reason for this:
            // By creating a byte array either dynamically or statically without necessarily calling any of the previous methods, or by modifying an existing byte array.
            // To prevent positive values from being misinterpreted as negative values, you can add a zero-byte value to the end of the array.
            // You can read more here: https://docs.microsoft.com/en-us/dotnet/api/system.numerics.biginteger.-ctor
            // So, the last byte is always zero-byte.
            var encryptedBytes = new byte[129];
            Array.Copy(packet.Buffer, 5, encryptedBytes, 0, packet.Length - 5);

            EncyptedNumber = new BigInteger(encryptedBytes);
        }
    }
}
