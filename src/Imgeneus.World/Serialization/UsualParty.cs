using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class UsualParty : BaseSerializable
    {
        [FieldOrder(0)]
        public byte LeaderIndex;

        [FieldOrder(1)]
        public byte Count;

        [FieldOrder(2)]
        [FieldCount(nameof(Count))]
        public List<PartyMember> Members { get; } = new List<PartyMember>();

        public UsualParty(IEnumerable<Character> partyMembers, byte leaderIndex)
        {
            LeaderIndex = leaderIndex;

            foreach (var member in partyMembers)
            {
                Members.Add(new PartyMember(member));
            }
        }

    }
}
