using Imgeneus.Network.Data;

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
            Message = packet.ReadString(messageLength - 1);
        }
    }
}