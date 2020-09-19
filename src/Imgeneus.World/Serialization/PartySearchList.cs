using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class PartySearchList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count;

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<PartySearchUnit> Members { get; } = new List<PartySearchUnit>();

        public PartySearchList(IEnumerable<Character> characters)
        {
            foreach (var c in characters)
                Members.Add(new PartySearchUnit(c));
        }
    }
}
