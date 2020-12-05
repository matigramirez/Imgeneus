using Imgeneus.Network.Data;

namespace Imgeneus.Network.Packets.Game
{
    public struct RenameCharacterPacket : IDeserializedPacket
    {
        public int CharacterId;
        public string NewName;

        public RenameCharacterPacket(IPacketStream packet)
        {
            CharacterId = packet.Read<int>();
            NewName = packet.ReadString(21);
        }

        public void Deconstruct(out int characterId, out string newName)
        {
            characterId = CharacterId;
            newName = NewName;
        }
    }
}