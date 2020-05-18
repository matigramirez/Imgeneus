using BinarySerialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.Network.Serialization
{
    public class CharacterDetails : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort Strength { get; }

        [FieldOrder(1)]
        public ushort Dexterity { get; }

        [FieldOrder(2)]
        public ushort Rec { get; }

        [FieldOrder(3)]
        public ushort Intelligence { get; }

        [FieldOrder(4)]
        public ushort Wisdom { get; }

        [FieldOrder(5)]
        public ushort Luck { get; }

        [FieldOrder(6)]
        public ushort StatPoint { get; }

        [FieldOrder(7)]
        public ushort SkillPoint { get; }

        [FieldOrder(8)]
        public int MaxHP { get; }

        [FieldOrder(9)]
        public int MaxMP { get; }

        [FieldOrder(10)]
        public int MaxSP { get; }

        [FieldOrder(11)]
        public ushort Angle { get; }

        [FieldOrder(12)]
        public uint StartLvlExp { get => 100; } // EXP Values are multiplied by 10

        [FieldOrder(13)]
        public uint EndLvlExp { get => 250; } // Next EXP is at 2500. Client takes the previous value, calculates the difference = 1500

        [FieldOrder(14)]
        public uint CurrentExp { get => 120; } //_character.Exp; } Current EXP is at 1200

        [FieldOrder(15)]
        public uint Gold { get; }

        [FieldOrder(16)]
        public float PosX { get; }

        [FieldOrder(17)]
        public float PosY { get; }

        [FieldOrder(18)]
        public float PosZ { get; }

        [FieldOrder(19)]
        public uint Kills { get; }

        [FieldOrder(20)]
        public uint Deaths { get; }

        [FieldOrder(21)]
        public uint Victories { get; }

        [FieldOrder(22)]
        public uint Defeats { get; }

        [FieldOrder(23)]
        public string GuildName { get => "TestGuildName"; }

        public CharacterDetails(Character character)
        {
            Strength = character.Strength;
            Dexterity = character.Dexterity;
            Rec = character.Rec;
            Intelligence = character.Intelligence;
            Wisdom = character.Wisdom;
            Luck = character.Luck;
            StatPoint = character.StatPoint;
            SkillPoint = character.SkillPoint;
            Angle = character.Angle;
            Gold = character.Gold;
            PosX = character.PosX;
            PosY = character.PosY;
            PosZ = character.PosZ;
            Kills = character.Kills;
            Deaths = character.Deaths;
            Victories = character.Victories;
            Defeats = character.Defeats;
            MaxHP = character.MaxHP;
            MaxMP = character.MaxMP;
            MaxSP = character.MaxSP;
        }
    }
}
