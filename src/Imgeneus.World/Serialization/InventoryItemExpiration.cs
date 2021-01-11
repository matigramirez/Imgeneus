using System;
using BinarySerialization;
using Imgeneus.Core.Extensions;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class InventoryItemExpiration : BaseSerializable
    {
        [FieldOrder(0)]
        public byte Bag { get; }

        [FieldOrder(1)]
        public byte Slot { get; }

        [FieldOrder(2)]
        public int CreationTime { get; }

        [FieldOrder(3)]
        public int ExpirationTime { get; }

        [FieldOrder(4)]
        public int Unknown { get; } = 0;

        public InventoryItemExpiration(Item item)
        {
            Bag = item.Bag;
            Slot = item.Slot;
            CreationTime = item.CreationTime.ToShaiyaTime();
            ExpirationTime = ((DateTime)item.ExpirationTime).ToShaiyaTime();
        }
    }
}
