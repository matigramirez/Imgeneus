using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class TradeItem : BaseSerializable
    {
        [FieldOrder(0)]
        public byte SlotInTradeWindow { get; }

        [FieldOrder(1)]
        public byte Type { get; }

        [FieldOrder(2)]
        public byte TypeId { get; }

        [FieldOrder(3)]
        public byte Quantity { get; }

        [FieldOrder(4)]
        public ushort Quality { get; }

        [FieldOrder(5)]
        public byte[] UnknownBytes { get; }

        [FieldOrder(6)]
        public int[] Gems { get; }

        [FieldOrder(7)]
        public CraftName CraftName { get; }

        [FieldOrder(8)]
        public byte UnknownByte { get; } // maybe part of craft name... not sure

        public TradeItem(byte slotInTradeWindow, byte quantity, Item item)
        {
            SlotInTradeWindow = slotInTradeWindow;
            Type = item.Type;
            TypeId = item.TypeId;
            Quantity = quantity;
            Gems = new int[] {
                item.Gem1 is null ? 0 : item.Gem1.TypeId,
                item.Gem2 is null ? 0 : item.Gem2.TypeId,
                item.Gem3 is null ? 0 : item.Gem3.TypeId,
                item.Gem4 is null ? 0 : item.Gem4.TypeId,
                item.Gem5 is null ? 0 : item.Gem5.TypeId,
                item.Gem6 is null ? 0 : item.Gem6.TypeId,
            };
            UnknownBytes = new byte[0];

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

        }
    }
}
