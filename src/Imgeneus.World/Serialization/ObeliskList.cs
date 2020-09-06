using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Zone.Obelisks;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class ObeliskList : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<ObeliskUnit> Obelisks { get; } = new List<ObeliskUnit>();

        public ObeliskList(IEnumerable<Obelisk> obelisks)
        {
            foreach (var obelisk in obelisks)
                Obelisks.Add(new ObeliskUnit(obelisk));
        }
    }
}
