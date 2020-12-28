using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class PartyMemberLevelChange : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public ushort Level;

        public PartyMemberLevelChange(Character character)
        {
            CharacterId = character.Id;
            Level = character.Level;
        }
    }
}