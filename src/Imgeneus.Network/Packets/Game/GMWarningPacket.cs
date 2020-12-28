using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GMWarningPacket : IDeserializedPacket
    {
        public string Name;
        public string Message;

        public GMWarningPacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);

            var messageLength = packet.Read<byte>();
            Message = packet.ReadString(messageLength);
        }
    }
}