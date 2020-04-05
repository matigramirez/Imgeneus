using BinarySerialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.Network.Serialization
{
    public class SerializedMovedItem : BaseSerializable
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
        public byte[] ItemDyed { get; }

        [FieldOrder(7)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(8)]
        public int[] Gems { get; }

        [FieldOrder(9)]
        public byte[] UnknownBytes2 { get; }

        public SerializedMovedItem(Item item)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            Type = item.Type;
            TypeId = item.TypeId;
            Count = item.Count;
            Quality = item.Quality;
            Gems = new int[] { item.GemTypeId1, item.GemTypeId2, item.GemTypeId3, item.GemTypeId4, item.GemTypeId5, item.GemTypeId6 };

            // Something connect with dyed feature. Couldn't figure out this this yet.
            // If all set to 1, you will see "Item dyed" string.
            ItemDyed = new byte[24];
            for (var i = 0; i < 24; i++)
            {
                ItemDyed[i] = 1;
            }

            UnknownBytes = new byte[26];
            for (var i = 0; i < 26; i++)
            {
                UnknownBytes[i] = 1;
            }

            UnknownBytes2 = new byte[21];
            for (var i = 0; i < 21; i++)
            {
                UnknownBytes2[i] = 1;
            }
        }
    }
}
