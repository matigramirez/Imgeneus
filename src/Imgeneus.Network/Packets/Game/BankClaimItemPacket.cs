using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct BankClaimItemPacket : IDeserializedPacket
    {
        public byte Slot;

        public BankClaimItemPacket(IPacketStream packet)
        {
            Slot = packet.Read<byte>();
        }
    }
}
