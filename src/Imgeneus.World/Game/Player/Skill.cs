using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Player
{
    public class Skill
    {
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
        public TypeDetail Type;

        /// <summary>
        /// To what target this skill can be applied.
        /// </summary>
        public TargetType TargetType;

        /// <summary>
        /// Time after which skill can be used again.
        /// </summary>
        public ushort ResetTime;

        /// <summary>
        /// Time for example for buffs. This time shows how long the skill will be applied.
        /// </summary>
        public int KeepTime;

        /// <summary>
        /// How long character should wait until skill is casted. In milliseconds.
        /// </summary>
        public int CastTime;

        /// <summary>
        /// How much stamina is needed in order to use this skill.
        /// </summary>
        public ushort NeedSP;

        /// <summary>
        /// How much mana is needed in order to use this skill.
        /// </summary>
        public ushort NeedMP;

        /// <summary>
        /// Creates skill from database inrofmation.
        /// </summary>
        public static Skill FromDbSkill(DbCharacterSkill dbSkill)
        {
            var skill = new Skill()
            {
                SkillId = dbSkill.Skill.SkillId,
                SkillLevel = dbSkill.Skill.SkillLevel,
                Number = dbSkill.Number,
                CooldownInSeconds = 0,
                Type = dbSkill.Skill.TypeDetail,
                TargetType = dbSkill.Skill.TargetType,
                ResetTime = dbSkill.Skill.ResetTime,
                KeepTime = dbSkill.Skill.KeepTime,
                CastTime = dbSkill.Skill.ReadyTime * 250,
                NeedSP = dbSkill.Skill.SP,
                NeedMP = dbSkill.Skill.MP
            };

            return skill;
        }
    }
}
