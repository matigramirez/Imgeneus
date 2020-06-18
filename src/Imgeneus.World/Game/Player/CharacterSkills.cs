using Imgeneus.Database.Constants;
using Imgeneus.DatabaseBackgroundService.Handlers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable
    {
        /// <summary>
        /// Collection of available skills.
        /// </summary>
        public Dictionary<int, Skill> Skills { get; private set; } = new Dictionary<int, Skill>();

        /// <summary>
        /// Event, that is fired, when character uses any skill.
        /// </summary>
        public event Action<Character, IKillable, Skill, AttackResult> OnUsedSkill;

        /// <summary>
        /// Event, that is fired, when character uses only range skill.
        /// </summary>
        public event Action<Character, IKillable, Skill, AttackResult> OnUsedRangeSkill;

        /// <summary>
        /// ?
        /// </summary>
        public event Action<Character, ActiveBuff, AttackResult> OnSkillKeep;

        /// <summary>
        /// Player learns new skill.
        /// </summary>
        /// <param name="skillId">skill id</param>
        /// <param name="skillLevel">skill level</param>
        /// <returns>successful or not</returns>
        public void LearnNewSkill(ushort skillId, byte skillLevel)
        {
            if (Skills.Values.Any(s => s.SkillId == skillId && s.SkillLevel == skillLevel))
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
            var isSkillLearned = Skills.Values.FirstOrDefault(s => s.SkillId == skillId);
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
                    skillNumber = Skills.Values.Select(s => s.Number).Max();
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
            if (isSkillLearned != null) Skills.Remove(skillNumber);

            SkillPoint -= dbSkill.SkillPoint;
            var skill = new Skill(dbSkill, skillNumber, 0);
            Skills.Add(skillNumber, skill);

            if (Client != null)
                _packetsHelper.SendLearnedNewSkill(Client, skill);

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
                    buff = AddActiveBuff(skill, this);
                    OnUsedRangeSkill?.Invoke(this, this, skill, new AttackResult());
                    break;

                case TargetType.SelectedEnemy:
                    buff = target.AddActiveBuff(skill, this);
                    OnUsedRangeSkill?.Invoke(this, target, skill, new AttackResult());
                    break;

                case TargetType.PartyMembers:
                    buff = AddActiveBuff(skill, this);
                    OnUsedRangeSkill?.Invoke(this, this, skill, new AttackResult());

                    if (Party != null)
                    {
                        foreach (var member in Party.Members)
                        {
                            buff = member.AddActiveBuff(skill, this);
                            OnUsedRangeSkill?.Invoke(this, member, skill, new AttackResult());
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException("Not implemented skill target.");
            }
        }

        /// <summary>
        /// Calculates healing result.
        /// </summary>
        private AttackResult UsedHealingSkill(Skill skill, IKillable target)
        {
            var healHP = TotalWis * 4 + skill.HealHP;
            var healSP = skill.HealSP;
            var healMP = skill.HealMP;
            AttackResult result = new AttackResult(AttackSuccess.Normal, new Damage((ushort)healHP, healSP, healMP));

            switch (skill.TargetType)
            {
                case TargetType.Caster:
                    CurrentHP += healHP;
                    CurrentMP += healMP;
                    CurrentSP += healSP;
                    break;

                case TargetType.AlliesButCaster:
                    if (Party != null)
                    {
                        foreach (var member in Party.Members.Where(m => m != this))
                        {
                            member.CurrentHP += healHP;
                            member.CurrentMP += healMP;
                            member.CurrentSP += healSP;

                            OnUsedSkill?.Invoke(this, member, skill, result);
                        }
                    }
                    break;

                case TargetType.SelectedEnemy:
                    target.IncreaseHP(healHP);
                    target.CurrentMP += healMP;
                    target.CurrentSP += healSP;
                    break;

                default:
                    throw new NotImplementedException("Not implemented skill target.");
            }

            return result;
        }

        /// <summary>
        /// Makes target invisible.
        /// </summary>
        private AttackResult UsedStealthSkill(Skill skill, IKillable target)
        {
            target.AddActiveBuff(skill, this);
            return new AttackResult(AttackSuccess.Normal, new Damage());
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
