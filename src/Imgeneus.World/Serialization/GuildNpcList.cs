using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class GuildNpcList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<GuildNpcUnit> Items { get; } = new List<GuildNpcUnit>();

        public GuildNpcList(IEnumerable<DbGuildNpcLvl> npcs)
        {
            foreach (var npc in npcs)
                Items.Add(new GuildNpcUnit(npc));
        }
    }
}
