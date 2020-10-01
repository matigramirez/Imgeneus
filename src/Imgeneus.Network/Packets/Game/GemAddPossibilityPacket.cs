using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct GemAddPossibilityPacket : IDeserializedPacket
    {
        public byte GemBag;
        public byte GemSlot;
        public byte DestinationBag;
        public byte DestinationSlot;
        public byte HammerBag;
        public byte HammerSlot;

        public GemAddPossibilityPacket(IPacketStream packet)
        {
            GemBag = packet.Read<byte>();
            GemSlot = packet.Read<byte>();
            DestinationBag = packet.Read<byte>();
            DestinationSlot = packet.Read<byte>();
            HammerBag = packet.Read<byte>();
            HammerSlot = packet.Read<byte>();
        }
    }
}
