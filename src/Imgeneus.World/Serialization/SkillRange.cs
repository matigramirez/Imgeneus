using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class SkillRange : BaseSerializable
    {
        [FieldOrder(0)]
        public bool IsSuccess { get; }

        [FieldOrder(1)]
        public int CharacterId { get; }

        [FieldOrder(2)]
        public int TargetId { get; }

        [FieldOrder(3)]
        public ushort SkillId { get; }

        [FieldOrder(4)]
        public byte SkillLevel { get; }

        [FieldOrder(5)]
        public ushort[] Damage = new ushort[3];

        public SkillRange(bool isSuccess, int characterId, int targetId, Skill skill, ushort[] damage)
        {
            IsSuccess = isSuccess;
            CharacterId = characterId;
            TargetId = targetId;
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
            Damage = damage;
        }
    }
}
