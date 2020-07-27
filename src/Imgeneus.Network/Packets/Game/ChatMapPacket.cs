using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatMapPacket : IDeserializedPacket
    {
        public string Message;

        public ChatMapPacket(IPacketStream packet)
        {
            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}
