using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterEnteredMapPacket : IDeserializedPacket
    {
        public CharacterEnteredMapPacket(IPacketStream packet)
        {
            // Doesn't contain any inforation, jus notification, that map was loaded on client side.
        }
    }
}
