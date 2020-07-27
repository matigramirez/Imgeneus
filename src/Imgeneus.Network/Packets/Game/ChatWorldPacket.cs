using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatWorldPacket : IDeserializedPacket
    {
        public string Message;

        public ChatWorldPacket(IPacketStream packet)
        {
            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}
