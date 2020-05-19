using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct CharacterShapePacket : IDeserializedPacket
    {
        public int CharacterId { get; set; }

        public CharacterShapePacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
        }
    }
}
