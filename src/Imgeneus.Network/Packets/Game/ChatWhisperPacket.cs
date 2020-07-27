using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChatWhisperPacket : IDeserializedPacket
    {
        public string TargetName;

        public string Message;

        public ChatWhisperPacket(IPacketStream packet)
        {
            TargetName = packet.ReadString(21);
            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}
