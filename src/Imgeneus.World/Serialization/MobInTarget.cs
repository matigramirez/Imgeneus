using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Serialization
{
    public class MobInTarget : BaseSerializable
    {
        [FieldOrder(0)]
        public int TargetId { get; }

        [FieldOrder(1)]
        public int CurrentHP { get; }

        public MobInTarget(Mob mob)
        {
            TargetId = mob.Id;
            CurrentHP = mob.CurrentHP;
        }
    }
}
