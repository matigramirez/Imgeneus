using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class FriendUnit : BaseSerializable
    {
        [FieldOrder(0)]
        public int Id;

        [FieldOrder(1)]
        public CharacterProfession Job;

        [FieldOrder(2)]
        public bool IsOnline;

        [FieldOrder(3), FieldLength(21)]
        public string Name;

        [FieldOrder(4)]
        public byte Length; // ?

        [FieldOrder(5)]
        public byte[] Memo = new byte[51]; // ?


        public FriendUnit(Friend friend)
        {
            Id = friend.Id;
            IsOnline = friend.IsOnline;
            Name = friend.Name;
            Job = friend.Job;
        }
    }
}