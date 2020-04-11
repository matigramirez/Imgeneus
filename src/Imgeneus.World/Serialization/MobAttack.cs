using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Serialization
{
    public class MobAttack : BaseSerializable
    {
        [FieldOrder(0)]
        public bool IsSuccess;

        [FieldOrder(1)]
        public uint MobId;

        [FieldOrder(2)]
        public int TargetId;

        [FieldOrder(3)]
        public ushort[] Damage;

        public MobAttack(Mob mob, int targetId)
        {
            IsSuccess = true; // I assume it's critical or not critical hit.
            MobId = mob.GlobalId;
            TargetId = targetId;
            Damage = new ushort[3]; // TODO: write damage here.
        }
    }
}
