using Imgeneus.Network.Data;
using System.Text;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatWhisperPacket : IDeserializedPacket
    {
        public string TargetName;

        public string Message;

        public ChatWhisperPacket(IPacketStream packet)
        {
            TargetName = packet.ReadString(21);

#if EP8_V2
            var length0 = packet.Read<byte>();
#endif

            var messageLength = packet.Read<byte>();

#if EP8_V2 || SHAIYA_US
            Message = packet.ReadString(messageLength, Encoding.Unicode);
#else
            Message = packet.ReadString(messageLength);
#endif
        }
    }
}
