using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class CharacterLevelUp : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId { get; }
        [FieldOrder(1)]
        public ushort Level { get; }
        [FieldOrder(2)]
        public ushort StatPoint { get; }
        [FieldOrder(3)]
        public ushort SkillPoint { get; }
        [FieldOrder(4)]
        public uint MinLevelExp { get; }
        [FieldOrder(5)]
        public uint NextLevelExp { get; }

        public CharacterLevelUp(Character character)
        {
            CharacterId = character.Id;
            Level = character.Level;
            StatPoint = character.StatPoint;
            SkillPoint = character.SkillPoint;
            MinLevelExp = character.MinLevelExp / 10; // Normalize experience for ep8 game
            NextLevelExp = character.NextLevelExp / 10; // Normalize experience for ep8 game
        }
    }
}