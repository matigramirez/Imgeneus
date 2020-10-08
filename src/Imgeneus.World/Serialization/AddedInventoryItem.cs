using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class AddedInventoryItem : BaseSerializable
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
        public int UnknownInt { get; }

        [FieldOrder(7)]
        public int[] Gems { get; }

        [FieldOrder(8)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(9)]
        public bool IsItemDyed { get; }

        [FieldOrder(10)]
        public byte[] UnknownBytes2 { get; }

        [FieldOrder(11)]
        public CraftName CraftName { get; }

        public AddedInventoryItem(Item item)
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
