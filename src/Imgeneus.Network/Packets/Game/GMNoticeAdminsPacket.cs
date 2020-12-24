using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMNoticeAdminsPacket : IDeserializedPacket
    {
        public string Message;

        public GMNoticeAdminsPacket(IPacketStream packet)
        {
            var messageLength = packet.Read<byte>();
            // Message always ends with an empty character
            Message = packet.ReadString(messageLength - 1);
        }
    }
}