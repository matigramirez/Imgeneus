using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class SkillCasting : BaseSerializable
    {
        [FieldOrder(0)]
        public int CharacterId { get; }

        [FieldOrder(1)]
        public int TargetId { get; }

        [FieldOrder(2)]
        public ushort SkillId { get; }

        [FieldOrder(3)]
        public byte SkillLevel { get; }

        public SkillCasting(int characterId, int targetId, Skill skill)
        {
            CharacterId = characterId;
            TargetId = targetId;
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
        }
    }
}
