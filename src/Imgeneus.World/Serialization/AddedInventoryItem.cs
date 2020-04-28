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
        public int[] Gems { get; }

        [FieldOrder(7)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(8)]
        public CraftName CraftName { get; }

        [FieldOrder(9)]
        public byte UnknownByte { get; } // maybe part of craft name... not sure

        public AddedInventoryItem(Item item)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            Type = item.Type;
            TypeId = item.TypeId;
            Count = item.Count;
            Quality = item.Quality;
            Gems = new int[] { item.GemTypeId1, item.GemTypeId2, item.GemTypeId3, item.GemTypeId4, item.GemTypeId5, item.GemTypeId6 };

            CraftName = new CraftName(
               '0', '1', // str 1
               '0', '2', // dex 2
               '0', '3', // rec 3
               '0', '4', // int 4
               '0', '5', // wis 5
               '0', '6', // luc 6
               '0', '7', // hp 700
               '0', '8', // mp 800
               '0', '9', // sp 900
               '2', '0' // step 20
               );

            UnknownBytes = new byte[54];
        }
    }
}
