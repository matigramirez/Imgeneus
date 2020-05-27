using Imgeneus.Database.Constants;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable
    {
        /// <summary>
        /// Collection of available skills.
        /// </summary>
        public ObservableRangeCollection<Skill> Skills { get; private set; } = new ObservableRangeCollection<Skill>();

        /// <summary>
        /// Event, that is fired, when character uses any skill.
        /// </summary>
        public event Action<Character, IKillable, Skill, AttackResult> OnUsedSkill;

        /// <summary>
        /// Event, that is fired, when character adds some buff to another character.
        /// </summary>
        public event Action<Character, IKillable, ActiveBuff> OnAddedBuffToAnotherCharacter;

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
            var skill = new Skill(dbSkill, skillNumber, 0);
            Skills.Add(skill);
            _logger.LogDebug($"Character {Id} learned skill {skill.SkillId} of level {skill.SkillLevel}");
        }

        /// <summary>
        /// Process use of buff skill.
        /// </summary>
        /// <param name="skill">buff skill</param>
        private void UsedBuffSkill(Skill skill, IKillable target)
        {
            ActiveBuff buff;

            switch (skill.TargetType)
            {
                case TargetType.Caster:
                    buff = AddActiveBuff(skill);
                    OnAddedBuffToAnotherCharacter?.Invoke(this, this, buff);
                    break;

                case TargetType.SelectedEnemy:
                    buff = target.AddActiveBuff(skill);
                    OnAddedBuffToAnotherCharacter?.Invoke(this, target, buff);
                    break;

                case TargetType.PartyMembers:
                    buff = AddActiveBuff(skill);
                    OnAddedBuffToAnotherCharacter?.Invoke(this, this, buff);

                    if (Party != null)
                    {
                        foreach (var member in Party.Members)
                        {
                            buff = member.AddActiveBuff(skill);
                            OnAddedBuffToAnotherCharacter?.Invoke(this, member, buff);
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException("Not implemented skill target.");
            }

            OnUsedSkill?.Invoke(this, this, skill, new AttackResult(AttackSuccess.SuccessBuff, new Damage(0, 0, 0)));
        }

        /// <summary>
        ///  Process use of attack skill.
        /// </summary>
        /// <param name="skill">attack skill</param>
        private void UsedAttackSkill(Skill skill, IKillable target)
        {
            if (Target.IsDead)
            {
                return;
            }

            var result = CalculateDamage(target, skill.TypeAttack, skill);
            target.DecreaseHP(result.Damage.HP, this);
            target.CurrentSP -= result.Damage.SP;
            target.CurrentMP -= result.Damage.MP;

            OnUsedSkill?.Invoke(this, target, skill, result);
        }

        #region Hit chance modifiers

        /// <summary>
        /// Possibility to hit enemy gained from skills.
        /// </summary>
        private double _skillPhysicalHittingChance;

        /// <summary>
        /// Possibility to escape hit gained from skills.
        /// </summary>
        private double _skillPhysicalEvasionChance;

        /// <summary>
        /// Possibility to make critical hit.
        /// </summary>
        private double _skillCriticalHittingChance;

        /// <summary>
        /// Possibility to hit enemy gained from skills.
        /// </summary>
        private double _skillMagicHittingChance;

        /// <summary>
        /// Possibility to escape hit gained from skills.
        /// </summary>
        private double _skillMagicEvasionChance;

        #endregion
    }
}
