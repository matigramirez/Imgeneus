using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class Skill
    {
        /// <summary>
        /// Unique id. Is used in ative buffs and maybe other places.
        /// </summary>
        public int Id;

        /// <summary>
        /// Skill id.
        /// </summary>
        public ushort SkillId;

        /// <summary>
        /// Skill level.
        /// </summary>
        public byte SkillLevel;

        /// <summary>
        /// Number. This value client sends, when player used any skill.
        /// </summary>
        public byte Number;

        /// <summary>
        /// Countdown in seconds.
        /// </summary>
        public int CooldownInSeconds;

        /// <summary>
        /// Skill type.
        /// </summary>
        public TypeDetail Type { get; private set; }

        /// <summary>
        /// To what target this skill can be applied.
        /// </summary>
        public TargetType TargetType { get; private set; }

        /// <summary>
        /// Time after which skill can be used again.
        /// </summary>
        public ushort ResetTime { get; private set; }

        /// <summary>
        /// Time for example for buffs. This time shows how long the skill will be applied.
        /// </summary>
        public int KeepTime { get; set; }

        /// <summary>
        /// Creates skill from database inrofmation.
        /// </summary>
        public static Skill FromDbSkill(DbCharacterSkill dbSkill)
        {
            var skill = new Skill()
            {
                Id = dbSkill.SkillId,
                SkillId = dbSkill.Skill.SkillId,
                SkillLevel = dbSkill.Skill.SkillLevel,
                Number = dbSkill.Number,
                CooldownInSeconds = 0,
                Type = dbSkill.Skill.TypeDetail,
                TargetType = dbSkill.Skill.TargetType,
                ResetTime = dbSkill.Skill.ResetTime,
                KeepTime = dbSkill.Skill.KeepTime
            };

            return skill;
        }
    }
}
