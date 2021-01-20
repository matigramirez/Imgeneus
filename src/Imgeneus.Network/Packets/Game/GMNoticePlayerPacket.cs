using Imgeneus.Network.Data;
using System.Text;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMNoticePlayerPacket : IDeserializedPacket
    {
        public string TargetName;
        public short TimeInterval;
        public string Message;

        public GMNoticePlayerPacket(IPacketStream packet)
        {
            TargetName = packet.ReadString(21);
            TimeInterval = packet.Read<short>();
            var messageLength = packet.Read<byte>();
            // Message always ends with an empty character
#if EP8_V2
            Message = packet.ReadString(messageLength, Encoding.Unicode);
#else
            Message = packet.ReadString(messageLength);
#endif
        }
    }
}