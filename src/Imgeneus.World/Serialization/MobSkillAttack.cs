using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class MobSkillAttack : BaseSerializable
    {
        [FieldOrder(0)]
        public AttackSuccess IsSuccess;

        [FieldOrder(1)]
        public int MobId;

        [FieldOrder(2)]
        public int TargetId;

        [FieldOrder(3)]
        public byte AttackType = 1; // Unknown.

        [FieldOrder(4)]
        public ushort SkillId;

        [FieldOrder(5)]
        public byte SkillLevel;

        [FieldOrder(6)]
        public ushort[] Damage;

        public MobSkillAttack(Mob mob, int targetId, Skill skill, AttackResult attackResult)
        {
            IsSuccess = attackResult.Success;
            MobId = mob.Id;
            TargetId = targetId;
            SkillId = skill.SkillId;
            SkillLevel = skill.SkillLevel;
            Damage = new ushort[] { attackResult.Damage.HP, attackResult.Damage.SP, attackResult.Damage.MP };
        }
    }
}
