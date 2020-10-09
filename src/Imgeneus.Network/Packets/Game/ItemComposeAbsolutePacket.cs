using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ItemComposePacket : IDeserializedPacket
    {
        public byte RuneBag;
        public byte RuneSlot;
        public byte ItemBag;
        public byte ItemSlot;

        public ItemComposePacket(IPacketStream packet)
        {
            RuneBag = packet.Read<byte>();
            RuneSlot = packet.Read<byte>();
            ItemBag = packet.Read<byte>();
            ItemSlot = packet.Read<byte>();
        }
    }
}
