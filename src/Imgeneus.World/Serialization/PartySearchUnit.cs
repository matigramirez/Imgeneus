using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class PartySearchUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Level;

        [FieldOrder(1)]
        public CharacterProfession Job;

        [FieldOrder(2)]
        public byte[] Name;

        public PartySearchUnit(Character character)
        {
            Level = (byte)character.Level;
            Job = character.Class;
            Name = character.NameAsByteArray;
        }
    }
}
