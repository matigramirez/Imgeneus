using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class FriendsList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<FriendUnit> Friends { get; } = new List<FriendUnit>();

        public FriendsList(IEnumerable<Friend> friends)
        {
            foreach (var f in friends)
                Friends.Add(new FriendUnit(f));
        }
    }
}
