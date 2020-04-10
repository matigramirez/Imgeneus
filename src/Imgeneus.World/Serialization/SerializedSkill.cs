using BinarySerialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.Network.Serialization
{
    public class SerializedSkill : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort SkillId { get; }

        [FieldOrder(1)]
        public byte SkillLevel { get; }

        [FieldOrder(2)]
        public byte Number { get; }

        [FieldOrder(3)]
        public int CooldownInSeconds { get; }

        public SerializedSkill(Skill skill)
        {
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
            Number = skill.Number;
            CooldownInSeconds = skill.CooldownInSeconds;
        }
    }
}