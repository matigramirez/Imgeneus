using BinarySerialization;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization.EP_8_V2
{
    public class CharacterShape : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharId { get; }

        [FieldOrder(1)]
        public bool IsDead { get; }

        [FieldOrder(2)]
        public Motion Motion { get; }

        [FieldOrder(3)]
        public Fraction Country { get; }

        [FieldOrder(4)]
        public Race Race { get; }

        [FieldOrder(5)]
        public byte Hair { get; }

        [FieldOrder(6)]
        public byte Face { get; }

        [FieldOrder(7)]
        public byte Height { get; }

        [FieldOrder(8)]
        public CharacterProfession Class { get; }

        [FieldOrder(9)]
        public Gender Gender { get; }

        [FieldOrder(10)]
        public byte PartyDefinition { get; }

        [FieldOrder(11)]
        public Mode Mode { get; }

        [FieldOrder(12)]
        public int Kills { get; }

        [FieldOrder(13)]
        public EquipmentItem[] EquipmentItems { get; } = new EquipmentItem[17];

        [FieldOrder(14)]
        public byte[] UnknownBytes3 { get; } = new byte[9];

        [FieldOrder(15)]
        public bool[] EquipmentItemHasColor { get; } = new bool[17];

        [FieldOrder(16)]
        public int UnknownInt { get; }

        [FieldOrder(17)]
        public DyeColorSerialized[] EquipmentItemColor { get; } = new DyeColorSerialized[17];

        [FieldOrder(18)]
        public byte[] UnknownBytes2 { get; } = new byte[451];

        [FieldOrder(19)]
        public byte[] Name;

        [FieldOrder(20)]
        public byte[] Name2;

        [FieldOrder(21)]
        public byte[] UnknownBytes4 = new byte[29];

        [FieldOrder(22)]
        public byte[] GuildName = new byte[25];

        public CharacterShape(Character character)
        {
            CharId = character.Id;
            IsDead = character.IsDead;
            Motion = character.Motion;
            Country = character.Country;
            Race = character.Race;
            Hair = character.Hair;
            Face = character.Face;
            Height = character.Height;
            Class = character.Class;
            Gender = character.Gender;
            Mode = character.Mode;
            Kills = character.Kills;
            Name = character.NameAsByteArray;
            Name2 = character.NameAsByteArray; // not sure why, but server definitely sends name twice

            for (byte i = 0; i < 17; i++)
            {
                character.InventoryItems.TryGetValue((0, i), out var item);
                EquipmentItems[i] = new EquipmentItem(item);

                if (item != null)
                {
                    EquipmentItemHasColor[i] = item.DyeColor.IsEnabled;
                    if (item.DyeColor.IsEnabled)
                        EquipmentItemColor[i] = new DyeColorSerialized(item.DyeColor.Saturation, item.DyeColor.R, item.DyeColor.G, item.DyeColor.B);
                }
            }

            if (character.HasParty)
            {
                if (character.IsPartyLead)
                {
                    PartyDefinition = 2;
                }
                else
                {
                    PartyDefinition = 1;
                }
            }
            else
            {
                PartyDefinition = 0;
            }

            var chars = "0123456789012345678901234".ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                GuildName[i] = (byte)chars[i];
            }
        }
    }
}
