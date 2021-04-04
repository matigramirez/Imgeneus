using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;

namespace Imgeneus.World.Serialization
{
    public class GuildUpdateUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id;

        [FieldOrder(1)]
        public int Points;

        [FieldOrder(2)]
        public byte Rank;

        public GuildUpdateUnit(DbGuild guild)
        {
            Id = guild.Id;
            Points = guild.Points;
            Rank = guild.Rank;
        }
    }
}
