using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterAdditionalStats : BaseSerializable
    {
        [FieldOrder(0)]
        public int Strength { get; }

        [FieldOrder(1)]
        public int Rec { get; }

        [FieldOrder(2)]
        public int Intelligence { get; }

        [FieldOrder(3)]
        public int Wisdom { get; }

        [FieldOrder(4)]
        public int Dexterity { get; }

        [FieldOrder(5)]
        public int Luck { get; }

        [FieldOrder(6)]
        public int MinAttack { get; }

        [FieldOrder(7)]
        public int MaxAttack { get; }

        [FieldOrder(8)]
        public int MinMagicAttack { get; }

        [FieldOrder(9)]
        public int MaxMagicAttack { get; }

        [FieldOrder(10)]
        public int Defense { get; }

        [FieldOrder(11)]
        public int Resistance { get; }

        public CharacterAdditionalStats(Character character)
        {
            Strength = character.ExtraStr;
            Rec = character.ExtraRec;
            Intelligence = character.ExtraInt;
            Wisdom = character.ExtraWis;
            Dexterity = character.ExtraDex;
            Luck = character.ExtraLuc;
            Defense = character.Defense;
            Resistance = character.Resistance;
            MinAttack = character.MinAttack;
            MaxAttack = character.MaxAttack;
            MinMagicAttack = character.MinMagicAttack;
            MaxMagicAttack = character.MaxMagicAttack;
        }
    }
}
