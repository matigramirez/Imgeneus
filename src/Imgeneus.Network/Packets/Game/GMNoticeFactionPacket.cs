using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMNoticeFactionPacket : IDeserializedPacket
    {
        public short TimeInterval;
        public string Message;

        public GMNoticeFactionPacket(IPacketStream packet)
        {
            TimeInterval = packet.Read<short>();
            var messageLength = packet.Read<byte>();
            // Message always ends with an empty character
            Message = packet.ReadString(messageLength - 1);
        }
    }
}