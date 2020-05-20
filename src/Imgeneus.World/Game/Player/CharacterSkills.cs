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
            // TODO: refactor it!
            var skill = new Skill()
            {
                SkillId = dbSkill.SkillId,
                SkillLevel = dbSkill.SkillLevel,
                Number = skillNumber,
                CooldownInSeconds = 0,
                Type = dbSkill.TypeDetail,
                TargetType = dbSkill.TargetType,
                ResetTime = dbSkill.ResetTime,
                KeepTime = dbSkill.KeepTime,
                CastTime = dbSkill.ReadyTime,
                NeedSP = dbSkill.SP,
                NeedMP = dbSkill.MP,
                NeedWeapon1 = dbSkill.NeedWeapon1 == 1,
                NeedWeapon2 = dbSkill.NeedWeapon2 == 1,
                NeedWeapon3 = dbSkill.NeedWeapon3 == 1,
                NeedWeapon4 = dbSkill.NeedWeapon4 == 1,
                NeedWeapon5 = dbSkill.NeedWeapon5 == 1,
                NeedWeapon6 = dbSkill.NeedWeapon6 == 1,
                NeedWeapon7 = dbSkill.NeedWeapon7 == 1,
                NeedWeapon8 = dbSkill.NeedWeapon8 == 1,
                NeedWeapon9 = dbSkill.NeedWeapon9 == 1,
                NeedWeapon10 = dbSkill.NeedWeapon10 == 1,
                NeedWeapon11 = dbSkill.NeedWeapon11 == 1,
                NeedWeapon12 = dbSkill.NeedWeapon12 == 1,
                NeedWeapon13 = dbSkill.NeedWeapon13 == 1,
                NeedWeapon14 = dbSkill.NeedWeapon14 == 1,
                NeedWeapon15 = dbSkill.NeedWeapon15 == 1,
                NeedShield = dbSkill.NeedShield == 1,
            };
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

            var result = CalculateDamage(target, skill);
            target.DecreaseHP(result.Damage.HP, this);
            target.CurrentSP -= result.Damage.SP;
            target.CurrentMP -= result.Damage.MP;

            OnUsedSkill?.Invoke(this, target, skill, result);
        }
    }
}
