using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct MoveItemInInventoryPacket : IDeserializedPacket
    {
        public byte CurrentBag { get; }

        public byte CurrentSlot { get; }

        public byte DestinationBag { get; }

        public byte DestinationSlot { get; }

        public MoveItemInInventoryPacket(IPacketStream packet)
        {
            CurrentBag = packet.Read<byte>();
            CurrentSlot = packet.Read<byte>();
            DestinationBag = packet.Read<byte>();
            DestinationSlot = packet.Read<byte>();
        }
    }
}
