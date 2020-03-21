using BinarySerialization;
using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.Network.Serialization
{
    public class InventoryItems : BaseSerializable
    {
        [FieldOrder(0)]
        public byte ItemsCount { get; }

        [FieldOrder(1)]
        public byte[] Items { get; }

        public InventoryItems(ICollection<DbCharacterItems> items)
        {
            ItemsCount = (byte)items.Count;

            var serializedItems = new List<byte>();
            foreach (var charItm in items)
            {
                var serialized = new SerializedItem(charItm).Serialize();
                serializedItems.AddRange(serialized);
            }
            Items = serializedItems.ToArray();
        }
    }
}
