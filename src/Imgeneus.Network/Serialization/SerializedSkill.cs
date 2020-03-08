using BinarySerialization;
using Imgeneus.Database.Entities;

namespace Imgeneus.Network.Serialization
{
    public class SerializedSkill : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort SkillId { get; }

        [FieldOrder(1)]
        public byte SkillLevel { get; }

        [FieldOrder(2)]
        public byte UnknownByte { get; }

        [FieldOrder(3)]
        public int CooldownInSeconds { get; }

        public SerializedSkill(DbCharacterSkill character)
        {
            SkillId = character.Skill.SkillId;
            SkillLevel = character.Skill.SkillLevel;
            CooldownInSeconds = 0; // TODO: add cooldown to DbCharacterSkill.
        }
    }
}