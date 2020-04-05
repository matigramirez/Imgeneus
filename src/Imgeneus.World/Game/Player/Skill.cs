using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class Skill
    {
        public ushort SkillId;
        public byte SkillLevel;
        public int CooldownInSeconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbSkill"></param>
        /// <returns></returns>
        public static Skill FromDbSkill(DbSkill dbSkill)
        {
            var skill = new Skill()
            {
                SkillId = dbSkill.SkillId,
                SkillLevel = dbSkill.SkillLevel,
                CooldownInSeconds = 0
            };

            return skill;
        }
    }
}
