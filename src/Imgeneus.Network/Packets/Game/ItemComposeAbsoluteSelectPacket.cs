using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ItemComposeAbsoluteSelectPacket : IDeserializedPacket
    {
        public byte Option;
        public ItemComposeAbsoluteSelectPacket(IPacketStream packet)
        {
            Option = packet.Read<byte>();
        }
    }
}
