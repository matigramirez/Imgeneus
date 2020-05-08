using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class SerializedActiveBuff : BaseSerializable
    {
        [FieldOrder(0)]
        public uint Id;

        [FieldOrder(1)]
        public ushort SkillId;

        [FieldOrder(2)]
        public byte SkillLevel;

        [FieldOrder(3)]
        public int CountDownInSeconds;

        public SerializedActiveBuff(ActiveBuff buff)
        {
            Id = buff.Id;
            SkillId = buff.SkillId;
            SkillLevel = buff.SkillLevel;
            CountDownInSeconds = buff.CountDownInSeconds;
        }
    }
}
