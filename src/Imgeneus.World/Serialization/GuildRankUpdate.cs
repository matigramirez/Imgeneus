using BinarySerialization;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class GuildRankUpdate : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<GuildUpdateUnit> Items { get; } = new List<GuildUpdateUnit>();

        public GuildRankUpdate(IEnumerable<(int GuildId, int Points, byte Rank)> guilds)
        {
            foreach (var guild in guilds)
                Items.Add(new GuildUpdateUnit(guild));
        }
    }
}
