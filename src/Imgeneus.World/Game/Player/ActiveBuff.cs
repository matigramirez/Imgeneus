using Imgeneus.Database.Entities;
using System;

namespace Imgeneus.World.Game.Player
{
    public class ActiveBuff
    {
        private static uint Counter = 1;

        public uint Id { get; private set; }

        private object SyncObj = new object();

        public DateTime ResetTime { get; set; }

        public int CountDownInSeconds { get => (int)ResetTime.Subtract(DateTime.UtcNow).TotalSeconds; }

        public ushort SkillId { get; set; }

        public byte SkillLevel { get; set; }

        public ActiveBuff()
        {
            lock (SyncObj)
            {
                Id = Counter++;
            }
        }

        public static ActiveBuff FromDbCharacterActiveBuff(DbCharacterActiveBuff buff)
        {
            return new ActiveBuff()
            {
                ResetTime = buff.ResetTime,
                SkillId = buff.Skill.SkillId,
                SkillLevel = buff.Skill.SkillLevel
            };
        }
    }
}
