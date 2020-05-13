using Imgeneus.Database.Constants;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : ITargetable
    {
        /// <summary>
        /// Collection of available skills.
        /// </summary>
        public ObservableRangeCollection<Skill> Skills { get; private set; } = new ObservableRangeCollection<Skill>();

        /// <summary>
        /// Player learns new skill.
        /// </summary>
        /// <param name="skillId">skill id</param>
        /// <param name="skillLevel">skill level</param>
        /// <returns>successful or not</returns>
        public void LearnNewSkill(ushort skillId, byte skillLevel)
        {
            if (Skills.Any(s => s.SkillId == skillId && s.SkillLevel == skillLevel))
            {
                // Character has already learned this skill.
                // TODO: log it or throw exception?
                return;
            }

            // Find learned skill.
            var dbSkill = _databasePreloader.Skills[(skillId, skillLevel)];
            if (SkillPoint < dbSkill.SkillPoint)
            {
                // Not enough skill points.
                // TODO: log it or throw exception?
                return;
            }

            byte skillNumber = 0;

            // Find out if the character has already learned the same skill, but lower level.
            var isSkillLearned = Skills.FirstOrDefault(s => s.SkillId == skillId);
            // If there is skil of lower level => delete it.
            if (isSkillLearned != null)
            {
                _taskQueue.Enqueue(ActionType.REMOVE_SKILL,
                                    Id, isSkillLearned.SkillId, isSkillLearned.SkillLevel);

                skillNumber = isSkillLearned.Number;
            }
            // No such skill. Generate new number.
            else
            {
                if (Skills.Any())
                {
                    // Find the next skill number.
                    skillNumber = Skills.Select(s => s.Number).Max();
                    skillNumber++;
                }
                else
                {
                    // No learned skills at all.
                }
            }

            // Save char and learned skill.
            _taskQueue.Enqueue(ActionType.SAVE_SKILL,
                                Id, dbSkill.SkillId, dbSkill.SkillLevel, skillNumber, dbSkill.SkillPoint);

            // Remove previously learned skill.
            if (isSkillLearned != null) Skills.Remove(isSkillLearned);

            SkillPoint -= dbSkill.SkillPoint;
            var skill = new Skill()
            {
                SkillId = dbSkill.SkillId,
                SkillLevel = dbSkill.SkillLevel,
                Number = skillNumber,
                CooldownInSeconds = 0,
                Type = dbSkill.TypeDetail,
                TargetType = dbSkill.TargetType,
                ResetTime = dbSkill.ResetTime,
                KeepTime = dbSkill.KeepTime
            };
            Skills.Add(skill);
            _logger.LogDebug($"Character {Id} learned skill {skill.SkillId} of level {skill.SkillLevel}");
        }

        /// <summary>
        /// Make character use skill.
        /// </summary>
        /// <param name="skillNumber">unique number of skill; unique is per character(maybe?)</param>
        public (Skill Skill, AttackResult AttackResult) UseSkill(byte skillNumber)
        {
            var skill = Skills.First(s => s.Number == skillNumber);

            Damage damage = new Damage(0, 0, 0);
            // TODO: implement use of all skills.
            // For now, just for testing I'm implementing buff to character.
            if (skill.Type == TypeDetail.Buff && (skill.TargetType == TargetType.Caster || skill.TargetType == TargetType.PartyMembers))
            {
                var buff = AddActiveBuff(skill);
            }
            else
            {
                damage = new Damage(100, 50, 20);
            }

            return (skill, new AttackResult(AttackSuccess.Critical, damage));
        }

        /// <summary>
        /// Usual physical attack, "auto attack".
        /// </summary>
        public AttackResult UsualAttack()
        {
            Damage damage = new Damage(33, 0, 0);
            return new AttackResult(AttackSuccess.Normal, damage);
        }
    }
}
