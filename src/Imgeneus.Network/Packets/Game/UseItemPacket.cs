using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct UseItemPacket : IDeserializedPacket
    {
        public byte Bag { get; }

        public byte Slot { get; }

        public UseItemPacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
        }
    }
}
