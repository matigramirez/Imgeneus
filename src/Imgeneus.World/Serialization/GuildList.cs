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
        public List<SerializedGuildListItem> Items { get; } = new List<SerializedGuildListItem>();


        public GuildList(IEnumerable<DbGuild> guilds)
        {
            foreach (var guild in guilds)
                Items.Add(new SerializedGuildListItem(guild));
        }

    }
}
