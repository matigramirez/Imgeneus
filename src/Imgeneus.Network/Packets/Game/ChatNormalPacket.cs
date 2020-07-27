using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatNormalPacket : IDeserializedPacket
    {
        public string Message;

        public ChatNormalPacket(IPacketStream packet)
        {
            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}
