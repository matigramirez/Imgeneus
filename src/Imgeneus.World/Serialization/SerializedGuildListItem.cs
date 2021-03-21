using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using System.Text;

namespace Imgeneus.World.Serialization
{
    public class SerializedGuildListItem : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id;

        [FieldOrder(1), FieldLength(25)]
        public string Name;

        [FieldOrder(2), FieldLength(21)]
        public string MasterName;

        [FieldOrder(3), FieldLength(65)]
        public byte[] Message;

        [FieldOrder(4)]
        public byte Rank = 3;

        [FieldOrder(5)]
        public uint Points = 6;

        public SerializedGuildListItem(DbGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            MasterName = "TODO: master name";
            //Message = Encoding.Unicode.GetBytes("guild message"); // new eps
            Message = Encoding.UTF8.GetBytes("guild message"); // old eps
        }
    }
}
