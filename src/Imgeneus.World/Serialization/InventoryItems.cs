using BinarySerialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.Network.Serialization
{
    public class InventoryItems : BaseSerializable
    {
        [FieldOrder(0)]
        public byte ItemsCount { get; }

        [FieldOrder(1)]
        public byte[] Items { get; }

        public InventoryItems(IEnumerable<Item> items)
        {
            var serializedItems = new List<byte>();
            foreach (var charItm in items)
            {
                var serialized = new InventoryItem(charItm).Serialize();
                serializedItems.AddRange(serialized);
                ItemsCount++;
            }
            Items = serializedItems.ToArray();
        }
    }
}
