using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class GuildList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<GuildUnit> Items { get; } = new List<GuildUnit>();


        public GuildList(IEnumerable<DbGuild> guilds)
        {
            foreach (var guild in guilds)
                Items.Add(new GuildUnit(guild));
        }

    }
}
