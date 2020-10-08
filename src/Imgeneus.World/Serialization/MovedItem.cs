using BinarySerialization;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Serialization;

namespace Imgeneus.Network.Serialization
{
    public class MovedItem : BaseSerializable
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

        [FieldOrder(5)]
        public ushort Quality { get; }

        [FieldOrder(6)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(7)]
        public bool IsItemDyed { get; }

        [FieldOrder(8)]
        public byte[] UnknownBytes2 { get; }

        [FieldOrder(9)]
        public int[] Gems { get; }

        [FieldOrder(10)]
        public CraftName CraftName { get; }

        public MovedItem(Item item)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            Type = item.Type;
            TypeId = item.TypeId;
            Count = item.Count;
            Quality = item.Quality;
            Gems = new int[] {
                item.Gem1 is null ? 0 : item.Gem1.TypeId,
                item.Gem2 is null ? 0 : item.Gem2.TypeId,
                item.Gem3 is null ? 0 : item.Gem3.TypeId,
                item.Gem4 is null ? 0 : item.Gem4.TypeId,
                item.Gem5 is null ? 0 : item.Gem5.TypeId,
                item.Gem6 is null ? 0 : item.Gem6.TypeId,
            };

            IsItemDyed = item.DyeColor.IsEnabled;

            CraftName = new CraftName(item.GetCraftName());

            // Check InventoryItem.cs for more info.
            UnknownBytes = new byte[23];
            UnknownBytes2 = new byte[26];
        }
    }
}
