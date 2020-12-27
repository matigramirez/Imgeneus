using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterEnteredPortalPacket : IDeserializedPacket
    {
        public byte PortalId;

        public CharacterEnteredPortalPacket(IPacketStream packet)
        {
            PortalId = packet.Read<byte>();
        }
    }
}
