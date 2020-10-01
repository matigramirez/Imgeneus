using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GemRemovePacket : IDeserializedPacket
    {
        public byte Bag;
        public byte Slot;
        public bool ShouldRemoveSpecificGem;
        public byte GemPosition;
        public byte HammerBag;
        public byte HammerSlot;

        public GemRemovePacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
            ShouldRemoveSpecificGem = packet.Read<bool>();
            GemPosition = packet.Read<byte>();
            HammerBag = packet.Read<byte>();
            HammerSlot = packet.Read<byte>();
        }
    }
}
