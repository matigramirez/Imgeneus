using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class RemovedInventoryItem : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Bag { get; }

        [FieldOrder(1)]
        public byte Slot { get; }

        [FieldOrder(2)]
        public byte Type { get; }

        [FieldOrder(3)]
        public byte TypeId { get; }

        [FieldOrder(4)]
        public byte Count { get; }

        public RemovedInventoryItem(Item item, bool fullRemove)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            Type = fullRemove ? (byte)0 : item.Type;
            TypeId = fullRemove ? (byte)0 : item.TypeId;
            Count = fullRemove ? (byte)0 : item.Count;
        }
    }
}
