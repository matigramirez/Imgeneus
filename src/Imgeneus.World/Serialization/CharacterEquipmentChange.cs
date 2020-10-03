using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterEquipmentChange : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public byte Slot;

        [FieldOrder(2)]
        public byte Type;

        [FieldOrder(3)]
        public byte TypeId;

        [FieldOrder(4)]
        public byte EnchantLevel;

        [FieldOrder(5)]
        public bool HasColor;

        [FieldOrder(6)]
        public byte UnknownByte;

        [FieldOrder(7)]
        public DyeColorSerialized DyeColor;

        [FieldOrder(8)]
        public byte[] UnknownBytes = new byte[39];

        [FieldOrder(9)]
        public byte[] CloakInfo = new byte[6];

        public CharacterEquipmentChange(int characterId, byte slot, Item item)
        {
            CharacterId = characterId;
            Slot = slot;

            if (item != null)
            {
                Type = item.Type;
                TypeId = item.TypeId;
                EnchantLevel = 20; // TODO: implement enchant here.
                HasColor = item.DyeColor.IsEnabled;
                if (HasColor)
                    DyeColor = new DyeColorSerialized(item.DyeColor.Saturation, item.DyeColor.R, item.DyeColor.G, item.DyeColor.B);
            }
        }
    }
}
