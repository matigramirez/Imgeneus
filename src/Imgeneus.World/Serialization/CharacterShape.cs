using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Serialization
{
    public class CharacterShape : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharId { get; }

        [FieldOrder(1)]
        public bool IsDead { get; }

        [FieldOrder(2)]
        public byte Motion { get; }

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
        public byte[] EquipmentItems { get; }

        [FieldOrder(14)]
        public bool[] EquipmentItemHasColor { get; } = new bool[16]; // TODO: support colors

        [FieldOrder(15)]
        public int UnknownInt { get; }

        [FieldOrder(16)]
        public byte[] EquipmentItemColor { get; } = new byte[16 * 4]; // TODO: support colors. Each color is 4 bytes. Probably alfa, r, g, b.

        [FieldOrder(17)]
        public byte[] UnknownBytes { get; } = new byte[459];

        [FieldOrder(18)]
        public byte[] Name = new byte[21];

        [FieldOrder(19)]
        public byte[] Dummy = new byte[31]; // Probably guild name... Or I'm out of ideas.

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
            var chars = character.Name.ToCharArray(0, character.Name.Length);
            for (var i = 0; i < chars.Length; i++)
            {
                Name[i] = (byte)chars[i];
            }

            var equipmentItems = character.InventoryItems.Where(item => item.Bag == 0).ToList();
            var equipmentBytes = new List<byte>();
            for (var i = 0; i < 16; i++)
            {
                var item = equipmentItems.FirstOrDefault(itm => itm.Slot == i);
                equipmentBytes.AddRange(new EquipmentItem(item).Serialize());
            }
            EquipmentItems = equipmentBytes.ToArray();

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
        }
    }
}
