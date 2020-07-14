using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class MobAttack : BaseSerializable
    {
        [FieldOrder(0)]
        public AttackSuccess IsSuccess;

        [FieldOrder(1)]
        public int MobId;

        [FieldOrder(2)]
        public int TargetId;

        [FieldOrder(3)]
        public ushort[] Damage;

        public MobAttack(Mob mob, int targetId, AttackResult attackResult)
        {
            IsSuccess = attackResult.Success;
            MobId = mob.Id;
            TargetId = targetId;
            Damage = new ushort[] { attackResult.Damage.HP, attackResult.Damage.SP, attackResult.Damage.MP };
        }
    }
}
