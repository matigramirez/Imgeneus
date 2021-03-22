using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class GuildUserUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id { get; }

        [FieldOrder(1)]
        public byte Rank { get; }

        [FieldOrder(2)]
        public ushort Level { get; }

        [FieldOrder(3)]
        public CharacterProfession Job { get; }

        [FieldOrder(4), FieldLength(21)]
        public string Name { get; }

        public GuildUserUnit(DbCharacter member)
        {
            Id = member.Id;
            Rank = member.GuildRank;
            Level = member.Level;
            Job = member.Class;
            Name = member.Name;
        }
    }
}
