using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using System.Text;

namespace Imgeneus.World.Serialization
{
    public class GuildUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id;

        [FieldOrder(1), FieldLength(25)]
        public string Name;

        [FieldOrder(2), FieldLength(21)]
        public string MasterName;

#if EP8_V2 || SHAIYA_US
        [FieldOrder(3), FieldLength(130)]
        public byte[] Message;
#else
        [FieldOrder(3), FieldLength(65)]
        public byte[] Message;
#endif

        [FieldOrder(4)]
        public byte Rank;

        [FieldOrder(5)]
        public int Points;

        public GuildUnit(DbGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            MasterName = guild.Master.Name;
            Rank = guild.Rank;
            Points = guild.Points;

#if EP8_V2 || SHAIYA_US
            Message = Encoding.Unicode.GetBytes(guild.Message);
#else
            Message = Encoding.UTF8.GetBytes(guild.Message);
#endif
        }
    }
}
