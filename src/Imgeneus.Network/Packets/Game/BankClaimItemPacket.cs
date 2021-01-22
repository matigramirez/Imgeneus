using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct BankClaimItemPacket : IDeserializedPacket
    {
        public byte Slot;
        public int Unknown1;
        public int Unknown2;

        public BankClaimItemPacket(IPacketStream packet)
        {
            Slot = packet.Read<byte>();
            Unknown1 = packet.Read<int>();
            Unknown2 = packet.Read<int>();
        }
    }
}
