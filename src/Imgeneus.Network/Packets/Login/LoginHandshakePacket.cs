using Imgeneus.Network.Data;
using Imgeneus.Network.Packets.Game;
using System.Linq;
using System.Numerics;

namespace Imgeneus.Network.Packets.Login
{
    public struct LoginHandshakePacket : IDeserializedPacket
    {
        public BigInteger EncyptedNumber { get; }

        public LoginHandshakePacket(IPacketStream packet)
        {
            var encryptedBytes = packet.Buffer.Skip(5).ToArray(); // 128 bytes here

            // Again endian problem. Client sends big integer as small endian, but BigInteger in c# is big endian.
            // So what we need to do in this case: we should reverse array and add 0-byte as first byte.
            // From here: https://stackoverflow.com/questions/48372017/convert-byte-array-to-biginteger
            // Example: client sends [2, 20, 200] we trasform to [0, 200, 20, 2]
            byte[] rev = new byte[encryptedBytes.Length + 1];
            for (int i = 0, j = encryptedBytes.Length; j > 0; i++, j--)
                rev[j] = encryptedBytes[i];

            EncyptedNumber = new BigInteger(encryptedBytes);
        }
    }
}
