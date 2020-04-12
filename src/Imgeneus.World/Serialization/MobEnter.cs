using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Monster;

namespace Imgeneus.World.Serialization
{
    public class MobEnter : BaseSerializable
    {
        [FieldOrder(0)]
        public int GlobalId { get; }

        [FieldOrder(1)]
        public bool IsNew { get; }

        [FieldOrder(2)]
        public ushort MobId { get; }

        [FieldOrder(3)]
        public float PosX { get; }

        [FieldOrder(4)]
        public float PosZ { get; }

        public MobEnter(Mob mob)
        {
            GlobalId = mob.Id;
            IsNew = true; // TODO: find out when it can be false;
            MobId = mob.MobId;
            PosX = mob.PosX;
            PosZ = mob.PosZ;
        }
    }
}
