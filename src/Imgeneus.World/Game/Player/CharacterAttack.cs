using Imgeneus.Database.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable, IKiller
    {
        #region Target

        private BaseKillable _target;
        public BaseKillable Target
        {
            get => _target; set
            {
                if (_target != null)
                {
                    _target.OnBuffAdded -= Target_OnBuffAdded;
                    _target.OnBuffRemoved -= Target_OnBuffRemoved;
                }

                _target = value;

                if (_target != null)
                {
                    _target.OnBuffAdded += Target_OnBuffAdded;
                    _target.OnBuffRemoved += Target_OnBuffRemoved;
                    TargetChanged(Target);
                }
            }
        }

        private void Target_OnBuffAdded(IKillable sender, ActiveBuff buff)
        {
            SendTargetAddBuff(sender, buff);
        }

        private void Target_OnBuffRemoved(IKillable sender, ActiveBuff buff)
        {
            SendTargetRemoveBuff(sender, buff);
        }

        #endregion

        #region Casting skill

        /// <summary>
        /// The timer, that is starting skill after cast time.
        /// </summary>
        private Timer _castTimer = new Timer();

        /// <summary>
        /// Skill, that player tries to cast.
        /// </summary>
        private Skill _skillInCast;

        /// <summary>
        /// Target for which we are casting spell.
        /// </summary>
        private IKillable _targetInCast;

        /// <summary>
        /// Event, that is fired, when user starts casting.
        /// </summary>
        public event Action<Character, IKillable, Skill> OnSkillCastStarted;

        /// <summary>
        /// Starts casting.
        /// </summary>
        /// <param name="skill">skill, that we are casting</param>
        /// <param name="target">target for which, that we are casting</param>
        private void StartCasting(Skill skill, IKillable target)
        {
            if (!CanUseSkill(skill, target))
                return;

            _skillInCast = skill;
            _targetInCast = target;
            _castTimer.Interval = skill.CastTime;
            _castTimer.Start();
            OnSkillCastStarted?.Invoke(this, Target, skill);
        }

        /// <summary>
        /// When time for casting has elapsed.
        /// </summary>
        private void CastTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _castTimer.Stop();
            UseSkill(_skillInCast, _targetInCast);

            _skillInCast = null;
            _targetInCast = null;
        }

        #endregion

        #region Damage calculation

        /// <summary>
        /// I'm not sure how exactly in original server next attack time was implemented.
        /// For now, I'm implementing it as usual date time and increase it based on attack speed and casting time.
        /// </summary>
        private DateTime _nextAttackTime;

        /// <summary>
        /// Uses skill or auto attack.
        /// </summary>
        private void Attack(int skillNumber, IKillable target = null)
        {
            if (IsStealth)
            {
                var stealthBuff = ActiveBuffs.FirstOrDefault(b => _databasePreloader.Skills[(b.SkillId, b.SkillLevel)].TypeDetail == TypeDetail.Stealth);
                stealthBuff.CancelBuff();
            }

            if (skillNumber == 255)
            {
                AutoAttack();
            }
            else
            {
                if (!Skills.TryGetValue(skillNumber, out var skill))
                {
                    _logger.LogWarning($"Character {Id} tries to use nonexistent skill.");
                    return;
                }

                if (skill.CastTime == 0)
                {
                    UseSkill(skill, target);
                }
                else
                {
                    StartCasting(skill, target);
                }
            }
        }

        /// <summary>
        /// Make character use skill.
        /// </summary>
        private void UseSkill(Skill skill, IKillable target = null)
        {
            if (!skill.IsPassive)
                SendAttackStart();

            if (!skill.IsPassive && !CanAttack(skill.Number, target))
                return;

            _nextAttackTime = DateTime.UtcNow.AddMilliseconds(NextAttackTime);

            if (skill.NeedMP > 0 || skill.NeedSP > 0)
            {
                CurrentMP -= skill.NeedMP;
                CurrentSP -= skill.NeedSP;
                SendUseSMMP(skill.NeedMP, skill.NeedSP);
            }

            var targets = new List<IKillable>();
            switch (skill.TargetType)
            {
                case TargetType.None:
                case TargetType.Caster:
                    targets.Add(this);
                    break;

                case TargetType.SelectedEnemy:
                    if (target != null)
                        targets.Add(target);
                    else
                        targets.Add(this);
                    break;

                case TargetType.PartyMembers:
                    if (Party != null)
                        foreach (var member in Party.Members)
                            targets.Add(member);
                    else
                        targets.Add(this);
                    break;

                case TargetType.EnemiesNearTarget:
                    var enemies = Map.GetEnemies(this, target, skill.ApplyRange);
                    foreach (var e in enemies)
                        targets.Add(e);
                    break;

                default:
                    throw new NotImplementedException("Not implemented skill target.");
            }

            foreach (var t in targets)
            {
                if (t.IsDead)
                    continue;

                if (skill.TypeAttack != TypeAttack.Passive && !((IKiller)this).AttackSuccessRate(t, skill.TypeAttack, skill))
                {
                    if (target == t)
                        OnUsedSkill?.Invoke(this, t, skill, new AttackResult(AttackSuccess.Miss, new Damage(0, 0, 0)));
                    else
                        OnUsedRangeSkill?.Invoke(this, t, skill, new AttackResult(AttackSuccess.Miss, new Damage(0, 0, 0)));

                    continue;
                }

                var attackResult = CalculateAttackResult(skill, t);

                if (attackResult.Damage.HP > 0)
                    t.DecreaseHP(attackResult.Damage.HP, this);
                if (attackResult.Damage.SP > 0)
                    t.CurrentSP -= attackResult.Damage.SP;
                if (attackResult.Damage.MP > 0)
                    t.CurrentMP -= attackResult.Damage.MP;

                switch (skill.Type)
                {
                    case TypeDetail.Buff:
                    case TypeDetail.SubtractingDebuff:
                    case TypeDetail.PeriodicalHeal:
                    case TypeDetail.PeriodicalDebuff:
                    case TypeDetail.PreventAttack:
                    case TypeDetail.Immobilize:
                        t.AddActiveBuff(skill, this);

                        if (target == t || this == t)
                            OnUsedSkill?.Invoke(this, target, skill, attackResult);
                        else
                            OnUsedRangeSkill?.Invoke(this, t, skill, attackResult);
                        break;

                    case TypeDetail.Healing:
                        var result = UsedHealingSkill(skill, t);
                        if (target == t || this == t)
                            OnUsedSkill?.Invoke(this, target, skill, result);
                        else
                            OnUsedRangeSkill?.Invoke(this, t, skill, result);
                        break;

                    case TypeDetail.Stealth:
                        result = UsedStealthSkill(skill, t);
                        if (target == t || this == t)
                            OnUsedSkill?.Invoke(this, target, skill, result);
                        else
                            OnUsedRangeSkill?.Invoke(this, t, skill, result);
                        break;

                    case TypeDetail.UniqueHitAttack:
                        if (target == t || this == t)
                            OnUsedSkill?.Invoke(this, target, skill, attackResult);
                        else
                            OnUsedRangeSkill?.Invoke(this, t, skill, attackResult);
                        break;

                    case TypeDetail.PassiveDefence:
                    case TypeDetail.WeaponMastery:
                        t.AddActiveBuff(skill, this);
                        break;

                    default:
                        _logger.LogError($"Not implemented skill type {skill.Type}");
                        //throw new NotImplementedException("Not implemented skill type.");
                        break;
                }
            }
        }

        /// <summary>
        /// Calculates attack result based on skill type and target.
        /// </summary>
        private AttackResult CalculateAttackResult(Skill skill, IKillable target)
        {
            switch (skill.DamageType)
            {
                case DamageType.FixedDamage:
                    return new AttackResult(AttackSuccess.Normal, new Damage(skill.DamageHP, skill.DamageMP, skill.DamageSP));

                case DamageType.PlusExtraDamage:
                    return ((IKiller)this).CalculateDamage(target,
                                                           skill.TypeAttack,
                                                           AttackElement,
                                                           MinAttack,
                                                           MaxAttack,
                                                           MinMagicAttack,
                                                           MaxMagicAttack,
                                                           skill);

                default:
                    throw new NotImplementedException("Not implemented damage type.");
            }
        }

        /// <summary>
        /// Event, that is fired, when character uses auto attack.
        /// </summary>
        public event Action<Character, AttackResult> OnAutoAttack;

        /// <summary>
        /// Usual physical attack, "auto attack".
        /// </summary>
        private void AutoAttack()
        {
            SendAttackStart();

            if (!CanAttack(255, Target))
                return;

            _nextAttackTime = DateTime.UtcNow.AddMilliseconds(NextAttackTime);

            AttackResult result;
            if (!((IKiller)this).AttackSuccessRate(Target, TypeAttack.PhysicalAttack))
            {
                result = new AttackResult(AttackSuccess.Miss, new Damage());
                OnAutoAttack?.Invoke(this, result);
                return;
            }

            result = ((IKiller)this).CalculateDamage(Target,
                                                     TypeAttack.PhysicalAttack,
                                                     AttackElement,
                                                     MinAttack,
                                                     MaxAttack,
                                                     MinMagicAttack,
                                                     MaxMagicAttack);
            Target.DecreaseHP(result.Damage.HP, this);
            Target.CurrentSP -= result.Damage.SP;
            Target.CurrentMP -= result.Damage.MP;

            OnAutoAttack?.Invoke(this, result);
        }

        /// <summary>
        /// Checks if it's possible to attack target. (or use skill)
        /// </summary>
        private bool CanAttack(byte skillNumber, IKillable target)
        {
            if (skillNumber == 255 && DateTime.UtcNow < _nextAttackTime)
            {
                // TODO: send not enough elapsed time?
                _logger.Log(LogLevel.Debug, "Too fast attack.");
                return false;
            }

            if (skillNumber == 255 && (target is null || target.IsDead))
            {
                SendAutoAttackWrongTarget(target);
                return false;
            }

            if (skillNumber == 255 && ActiveBuffs.Any(b => b.StateType == StateType.Sleep || b.StateType == StateType.Stun || b.StateType == StateType.Silence))
            {
                SendAutoAttackCanNotAttack(target);
                return false;
            }

            if (skillNumber != 255)
            {
                if (!Skills.TryGetValue(skillNumber, out var skill))
                {
                    _logger.LogWarning($"Character {Id} tries to use nonexistent skill.");
                    return false;
                }

                return CanUseSkill(skill, target);
            }

            return true;
        }

        /// <summary>
        /// Checks if it's enough sp and mp in order to use a skill.
        /// </summary>
        /// <param name="skill">skill, that character wants to use</param>
        private bool CanUseSkill(Skill skill, IKillable target)
        {
            if (DateTime.UtcNow < _nextAttackTime)
            {
                SendCooldownNotOver(target, skill);
                return false;
            }

            if (skill.TargetType == TargetType.AnyEnemy || skill.TargetType == TargetType.EnemiesNearTarget &&
                (target is null || target.IsDead))
            {
                SendSkillWrongTarget(target, skill);
                return false;
            }

            if (Weapon is null || !skill.RequiredWeapons.Contains(Weapon.Type))
            {
                SendSkillWrongEquipment(target, skill);
                return false;
            }

            if (skill.NeedShield && Shield is null)
            {
                SendSkillWrongEquipment(target, skill);
                return false;
            }

            if (CurrentMP < skill.NeedMP || CurrentSP < skill.NeedSP)
            {
                SendNotEnoughMPSP(Target, skill);
                return false;
            }

            if ((skill.TypeAttack == TypeAttack.PhysicalAttack || skill.TypeAttack == TypeAttack.ShootingAttack) &&
                ActiveBuffs.Any(b => b.StateType == StateType.Sleep || b.StateType == StateType.Stun || b.StateType == StateType.Silence))
            {
                SendSkillAttackCanNotAttack(target, skill);
                return false;
            }

            if (skill.TypeAttack == TypeAttack.MagicAttack &&
                ActiveBuffs.Any(b => b.StateType == StateType.Sleep || b.StateType == StateType.Stun || b.StateType == StateType.Darkness))
            {
                SendSkillAttackCanNotAttack(target, skill);
                return false;
            }

            return true;
        }

        #endregion
    }
}
