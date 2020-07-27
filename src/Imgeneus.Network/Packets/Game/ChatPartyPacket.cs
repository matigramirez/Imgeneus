using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatPartyPacket : IDeserializedPacket
    {
        public string Message;

        public ChatPartyPacket(IPacketStream packet)
        {
            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}
