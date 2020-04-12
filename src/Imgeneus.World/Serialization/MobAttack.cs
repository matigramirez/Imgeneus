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
        public int MobId;

        [FieldOrder(2)]
        public int TargetId;

        [FieldOrder(3)]
        public ushort[] Damage;

        public MobAttack(Mob mob, int targetId)
        {
            IsSuccess = true; // I assume it's critical or not critical hit.
            MobId = mob.Id;
            TargetId = targetId;
            Damage = new ushort[3]; // TODO: write damage here.
            Damage[0] = 10; // Health damage
            Damage[1] = 5; // Stamina damage
            Damage[2] = 3; // Mana damage
        }
    }
}
