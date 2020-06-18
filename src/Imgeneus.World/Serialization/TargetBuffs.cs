using BinarySerialization;
using Imgeneus.Network.Serialization;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;

namespace Imgeneus.World.Serialization
{
    public class TargetBuffs : BaseSerializable
    {
        [FieldOrder(0)]
        public byte TargetType;

        [FieldOrder(1)]
        public int TargetId;

        [FieldOrder(2)]
        public byte BuffsCount;

        [FieldOrder(3)]
        [FieldCount(nameof(BuffsCount))]
        public List<TargetBuff> Buffs { get; } = new List<TargetBuff>();

        public TargetBuffs(IKillable target)
        {
            TargetId = target.Id;

            if (target is Mob)
                TargetType = 2;
            else
                TargetType = 1;

            foreach (var buff in target.ActiveBuffs)
            {
                Buffs.Add(new TargetBuff(buff));
            }
        }
    }

    public class TargetBuff : BaseSerializable
    {
        [FieldOrder(0)]
        public ushort SkillId;

        [FieldOrder(1)]
        public byte SkillLevel;

        [FieldOrder(2)]
        public int CountDownInSeconds;

        public TargetBuff(ActiveBuff buff)
        {
            SkillId = buff.SkillId;
            SkillLevel = buff.SkillLevel;
            CountDownInSeconds = buff.CountDownInSeconds;
        }
    }
}
