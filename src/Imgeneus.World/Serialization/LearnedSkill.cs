using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class LearnedSkill : BaseSerializable
    {
        [FieldOrder(0)]
        public bool IsFailed { get; }

        [FieldOrder(1)]
        public byte Number { get; }

        [FieldOrder(2)]
        public ushort SkillId { get; }

        [FieldOrder(3)]
        public byte SkillLevel { get; }

        public LearnedSkill(Skill skill)
        {
            IsFailed = false;
            Number = skill.Number;
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
        }
    }
}
