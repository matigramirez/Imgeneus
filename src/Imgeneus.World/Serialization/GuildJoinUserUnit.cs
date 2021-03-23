using Imgeneus.Network.Serialization;
using Imgeneus.Database.Entities;
using BinarySerialization;

namespace Imgeneus.World.Serialization
{
    public class GuildJoinUserUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id { get; }

        [FieldOrder(1)]
        public ushort Level { get; }

        [FieldOrder(2)]
        public CharacterProfession Job { get; }

        [FieldOrder(3), FieldLength(21)]
        public string Name { get; }

        public GuildJoinUserUnit(DbCharacter character)
        {
            Id = character.Id;
            Level = character.Level;
            Job = character.Class;
            Name = character.Name;
        }
    }
}
