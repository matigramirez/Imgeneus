using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct ChangeAppearancePacket : IDeserializedPacket
    {
        public byte Bag;

        public byte Slot;

        public byte Hair;

        public byte Face;

        public byte Size;

        public byte Sex;

        public ChangeAppearancePacket(IPacketStream packet)
        {
            Bag = packet.Read<byte>();
            Slot = packet.Read<byte>();
            Hair = packet.Read<byte>();
            Face = packet.Read<byte>();
            Size = packet.Read<byte>();
            Sex = packet.Read<byte>();
        }
    }
}
