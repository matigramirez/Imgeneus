using BinarySerialization;
using Imgeneus.Database.Entities;
using System;

namespace Imgeneus.Network.Serialization
{
    public class SerializedItem : BaseSerializable
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
        public ushort Quality { get; }

        [FieldOrder(5)]
        public int[] Gems { get; }

        [FieldOrder(6)]
        public byte Count { get; }

        [FieldOrder(7)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(8)]
        public byte[] FromDate { get; }

        [FieldOrder(9)]
        public byte[] UntilDate { get; }

        [FieldOrder(10)]
        public byte[] ItemDyed { get; }

        public SerializedItem(DbCharacterItems item)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            Type = item.Type;
            TypeId = item.TypeId;
            Quality = item.Quality;
            Gems = new int[] { item.GemTypeId1, item.GemTypeId2, item.GemTypeId3, item.GemTypeId4, item.GemTypeId5, item.GemTypeId6 };
            Count = item.Count;

            // Unknown bytes. Maybe enchant?
            UnknownBytes = new byte[27];
            for (var i = 0; i < 27; i++) // 9
            {
                UnknownBytes[i] = 1;
            }

            // 8 bytes, 4 per 1 date, but date is calculated wrong.
            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            byte[] dateBytes = BitConverter.GetBytes((int)now.Ticks);
            FromDate = dateBytes;
            UntilDate = dateBytes;

            // Something connect with dyed feature. Couldn't figure out this this yet.
            ItemDyed = new byte[36];
            for (var i = 0; i < 36; i++)
            {
                ItemDyed[i] = 1;
            }
        }
    }
}
