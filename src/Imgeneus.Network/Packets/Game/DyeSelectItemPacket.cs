using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct DyeSelectItemPacket : IDeserializedPacket
    {
        public byte DyeItemBag;
        public byte DyeItemSlot;
        public byte TargetItemBag;
        public byte TargetItemSlot;

        public DyeSelectItemPacket(IPacketStream packet)
        {
            DyeItemBag = packet.Read<byte>();
            DyeItemSlot = packet.Read<byte>();
            TargetItemBag = packet.Read<byte>();
            TargetItemSlot = packet.Read<byte>();
        }
    }
}

