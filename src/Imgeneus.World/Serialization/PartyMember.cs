using BinarySerialization;
using Imgeneus.Database.Entities;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class PartyMember : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId;

        [FieldOrder(1)]
        public byte[] Name = new byte[21];

        [FieldOrder(2)]
        public ushort Level;

        [FieldOrder(3)]
        public CharacterProfession Class;

        [FieldOrder(4)]
        public int MaxHP;

        [FieldOrder(5)]
        public int HP;

        [FieldOrder(6)]
        public int MaxSP;

        [FieldOrder(7)]
        public int SP;

        [FieldOrder(8)]
        public int MaxMP;

        [FieldOrder(9)]
        public int MP;

        [FieldOrder(10)]
        public ushort Map;

        [FieldOrder(11)]
        public float X;

        [FieldOrder(12)]
        public float Y;

        [FieldOrder(13)]
        public float Z;

        [FieldOrder(14)]
        public byte BuffsCount;

        [FieldOrder(15)]
        [FieldCount(nameof(BuffsCount))]
        public List<PartyMemberBuff> Buffs { get; } = new List<PartyMemberBuff>();

        public PartyMember(Character character)
        {
            CharacterId = character.Id;
            Level = character.Level;
            Class = character.Class;
            MaxHP = character.MaxHP;
            HP = character.CurrentHP;
            MaxSP = character.MaxSP;
            SP = character.CurrentSP;
            MaxMP = character.MaxMP;
            MP = character.CurrentMP;
            Map = character.Map;
            X = character.PosX;
            Y = character.PosY;
            Z = character.PosZ;

            var chars = character.Name.ToCharArray(0, character.Name.Length);
            for (var i = 0; i < chars.Length; i++)
            {
                Name[i] = (byte)chars[i];
            }

            foreach (var buff in character.ActiveBuffs)
            {
                Buffs.Add(new PartyMemberBuff(buff));
            }
        }

    }
}
