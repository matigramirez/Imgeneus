using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ItemComposeAbsolutePacket : IDeserializedPacket
    {
        public byte RuneBag;
        public byte RuneSlot;
        public byte ItemBag;
        public byte ItemSlot;

        public ItemComposeAbsolutePacket(IPacketStream packet)
        {
            RuneBag = packet.Read<byte>();
            RuneSlot = packet.Read<byte>();
            ItemBag = packet.Read<byte>();
            ItemSlot = packet.Read<byte>();
        }
    }
}
