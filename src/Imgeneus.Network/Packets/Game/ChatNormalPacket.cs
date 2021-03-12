using Imgeneus.Network.Data;
using System.Text;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatNormalPacket : IDeserializedPacket
    {
        public string Message;

        public ChatNormalPacket(IPacketStream packet)
        {
#if EP8_V2
            var length0 = packet.Read<byte>();
#endif
            var messageLength = packet.Read<byte>();

#if (EP8_V2 || SHAIYA_US)
            Message = packet.ReadString(messageLength, Encoding.Unicode);
#else
            Message = packet.ReadString(messageLength);
#endif
        }
    }
}
