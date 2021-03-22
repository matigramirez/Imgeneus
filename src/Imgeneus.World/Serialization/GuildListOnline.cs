using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class GuildListOnline : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<GuildUserUnit> Members { get; } = new List<GuildUserUnit>();

        public GuildListOnline(IEnumerable<DbCharacter> members)
        {
            foreach (var member in members)
                Members.Add(new GuildUserUnit(member));
        }
    }
}
