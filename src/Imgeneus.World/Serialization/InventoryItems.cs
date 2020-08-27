using BinarySerialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.Network.Serialization
{
    public class InventoryItems : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Count { get; }

        [FieldOrder(1)]
        [FieldCount(nameof(Count))]
        public List<InventoryItem> Items { get; } = new List<InventoryItem>();

        public InventoryItems(IEnumerable<Item> items)
        {
            foreach (var charItm in items)
                Items.Add(new InventoryItem(charItm));
        }
    }
}
