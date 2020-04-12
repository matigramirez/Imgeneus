using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Serialization
{
    public class MobMove : BaseSerializable
    {
        [FieldOrder(0)]
        public int GlobalId { get; }

        [FieldOrder(1)]
        public MobMotion Motion { get; }

        [FieldOrder(2)]
        public float PosX { get; }

        [FieldOrder(3)]
        public float PosZ { get; }

        public MobMove(Mob mob)
        {
            GlobalId = mob.Id;
            Motion = mob.MoveMotion;
            PosX = mob.PosX;
            PosZ = mob.PosZ;
        }
    }
}
