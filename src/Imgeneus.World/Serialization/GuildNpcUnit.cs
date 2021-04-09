using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class GuildNpcUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Type { get; }

        [FieldOrder(1)]
        public byte Group { get; }

        [FieldOrder(2)]
        public byte Level { get; }

        [FieldOrder(3)]
        public byte Number { get; } // This looks like iterator, but not used anywhere in clientside (as per UZC).

        public GuildNpcUnit(DbGuildNpcLvl npc)
        {
            Type = (byte)npc.NpcType;
            Group = npc.Group;
            Level = npc.NpcLevel;
        }
    }
}
