using Imgeneus.Database.Entities;
using System;

namespace Imgeneus.World.Game.Player
{
    public class ActiveBuff
    {
        public int Id { get; private set; }

        public DateTime ResetTime { get; set; }

        public int CountDownInSeconds { get => 100; } // TODO: implement countdown. It's reset time - now.

        public ushort SkillId { get; set; }

        public byte SkillLevel { get; set; }

        public static ActiveBuff FromDbCharacterActiveBuff(DbCharacterActiveBuff buff)
        {
            return new ActiveBuff()
            {
                Id = buff.Id,
                ResetTime = buff.ResetTime,
                SkillId = buff.Skill.SkillId,
                SkillLevel = buff.Skill.SkillLevel
            };
        }
    }
}
