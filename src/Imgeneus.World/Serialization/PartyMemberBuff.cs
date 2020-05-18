using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game.Player;

namespace Imgeneus.World.Serialization
{
    public class PartyMemberBuff : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort SkillId;

        [FieldOrder(1)]
        public byte SkillLevel;

        [FieldOrder(2)]
        public int CountDownInSeconds;

        public PartyMemberBuff(ActiveBuff buff)
        {
            SkillId = buff.SkillId;
            SkillLevel = buff.SkillLevel;
            CountDownInSeconds = buff.CountDownInSeconds;
        }
    }
}
