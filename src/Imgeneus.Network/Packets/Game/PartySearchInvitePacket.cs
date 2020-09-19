using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct PartySearchInvitePacket : IDeserializedPacket
    {
        public string Name;

        public PartySearchInvitePacket(IPacketStream packet)
        {
            Name = packet.ReadString(21);
        }
    }
}
