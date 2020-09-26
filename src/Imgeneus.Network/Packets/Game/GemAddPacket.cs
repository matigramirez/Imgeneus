using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GemAddPacket : IDeserializedPacket
    {
        public byte Bag;
        public byte Slot;
        public byte DestinationBag;
        public byte DestinationSlot;
        public byte HammerBag;
        public byte HammerSlot;

        public GemAddPacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
            DestinationBag = packet.Read<byte>();
            DestinationSlot = packet.Read<byte>();
            HammerBag = packet.Read<byte>();
            HammerSlot = packet.Read<byte>();
        }
    }
}
