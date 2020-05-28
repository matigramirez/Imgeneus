using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Monster;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable
    {
        #region Target

        private IKillable _target;
        public IKillable Target
        {
            get => _target; set
            {
                _target = value;

                if (_target != null)
                {
                    TargetChanged(Target);
                }
            }
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
            if (skillNumber == 255)
            {
                AutoAttack();
            }
            else
            {
                var skill = Skills.First(s => s.Number == skillNumber);

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
            SendAttackStart();

            if (!CanAttack(skill.Number, target))
                return;

            _nextAttackTime = DateTime.UtcNow.AddMilliseconds(NextAttackTime);

            CurrentMP -= skill.NeedMP;
            CurrentSP -= skill.NeedSP;
            SendUseSMMP(skill.NeedMP, skill.NeedSP);

            if (!AttackSuccessRate(target, skill.TypeAttack, skill))
            {
                OnUsedSkill?.Invoke(this, target, skill, new AttackResult(AttackSuccess.Miss, new Damage(0, 0, 0)));
                return;
            }

            AttackResult result;
            switch (skill.DamageType)
            {
                case DamageType.FixedDamage:
                    result = new AttackResult(AttackSuccess.Normal, new Damage(skill.DamageHP, skill.DamageMP, skill.DamageSP));
                    break;

                case DamageType.PlusExtraDamage:
                    result = CalculateDamage(target, skill.TypeAttack, skill);
                    break;

                default:
                    throw new NotImplementedException("Not implemented damage type.");
            }

            if (target != null && target.IsDead)
                return;

            if (target != null)
            {
                target.DecreaseHP(result.Damage.HP, this);
                target.CurrentSP -= result.Damage.SP;
                target.CurrentMP -= result.Damage.MP;
            }

            switch (skill.Type)
            {
                case TypeDetail.Buff:
                case TypeDetail.SubtractingDebuff:
                case TypeDetail.PeriodicalHeal:
                    UsedBuffSkill(skill, target);
                    break;

                case TypeDetail.Healing:
                    result = UsedHealingSkill(skill, target);
                    break;

                default:
                    throw new NotImplementedException("Not implemented skill type.");
            }

            OnUsedSkill?.Invoke(this, target, skill, result);
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
            if (!AttackSuccessRate(Target, TypeAttack.PhysicalAttack))
            {
                result = new AttackResult(AttackSuccess.Miss, new Damage());
                OnAutoAttack?.Invoke(this, result);
                return;
            }

            result = CalculateDamage(Target, TypeAttack.PhysicalAttack);
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

            if (skillNumber != 255)
            {
                return CanUseSkill(Skills.First(s => s.Number == skillNumber), target);
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

            return true;
        }

        /// <summary>
        /// Calculates damage based on player stats and target stats.
        /// </summary>
        private AttackResult CalculateDamage(IKillable target, TypeAttack typeAttack, Skill skill = null)
        {
            double damage = 0;

            // First, caculate damage, that is made of stats, weapon and buffs.
            switch (typeAttack)
            {
                case TypeAttack.PhysicalAttack:
                    damage = new Random().Next(MinAttack, MaxAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Defense;
                    if (damage < 0)
                        damage = 1;
                    damage = damage * 1.5;
                    break;

                case TypeAttack.ShootingAttack:
                    damage = new Random().Next(MinAttack, MaxAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Defense;
                    if (damage < 0)
                        damage = 1;
                    // TODO: multiply by range to the target.
                    damage = damage * 1.5; // * 0.7 if target is too close.
                    break;

                case TypeAttack.MagicAttack:
                    damage = new Random().Next(MinMagicAttack, MaxMagicAttack);
                    if (skill != null)
                    {
                        damage += skill.DamageHP;
                    }
                    damage -= target.Resistance;
                    if (damage < 0)
                        damage = 1;
                    damage = damage * 1.5;
                    break;
            }

            // Second, add element calculation.
            // TODO: add element calculation.

            // Third, caculate if critical damage should be added.
            var criticalDamage = false;
            if (new Random().Next(1, 101) < CriticalSuccessRate(target))
            {
                criticalDamage = true;
                damage += Convert.ToInt32(TotalLuc * new Random().NextDouble() * 1.5);
            }

            if (damage > 30000)
                damage = 30000;

            if (criticalDamage)
                return new AttackResult(AttackSuccess.Critical, new Damage(Convert.ToUInt16(damage), 0, 0));
            else
                return new AttackResult(AttackSuccess.Normal, new Damage(Convert.ToUInt16(damage), 0, 0));
        }

        private bool AttackSuccessRate(IKillable target, TypeAttack typeAttack, Skill skill = null)
        {
            if (skill != null && (skill.StateType == StateType.FlatDamage || skill.StateType == StateType.DeathTouch))
                return true;

            if (skill != null && skill.UseSuccessValue)
                return new Random().Next(1, 101) < skill.SuccessValue;


            double levelDifference;
            double result;

            // Starting from here there might be not clear code.
            // This code is not my invention, it's raw implementation of ep 4 calculations.
            // You're free to change it to whatever you think fits better your server.
            switch (typeAttack)
            {
                case TypeAttack.PhysicalAttack:
                case TypeAttack.ShootingAttack:
                    levelDifference = Level * 1.0 / (target.Level + Level);
                    var targetAttackPercent = target.PhysicalHittingChance / (target.PhysicalHittingChance + PhysicalEvasionChance);
                    var myAttackPercent = PhysicalHittingChance / (PhysicalHittingChance + target.PhysicalEvasionChance);
                    result = levelDifference * 160 - (targetAttackPercent * 100 - myAttackPercent * 100);
                    if (result >= 20)
                    {
                        if (result > 99)
                            result = 99;
                    }
                    else
                    {
                        if (target is Mob)
                            result = 20;
                        else
                            result = 1;
                    }

                    return new Random().Next(1, 101) < result;

                case TypeAttack.MagicAttack:
                    levelDifference = ((target.Level - Level - 2) * 100 + target.Level) / (target.Level + Level) * 1.1;
                    var fxDef = levelDifference + target.MagicEvasionChance;
                    if (fxDef >= 1)
                    {
                        if (fxDef > 70)
                            fxDef = 70;
                    }
                    else
                    {
                        fxDef = 1;
                    }

                    var wisDifference = (11 * target.TotalWis - 10 * TotalWis) / (target.TotalWis + TotalWis) * 3.9000001;
                    var nAttackTypea = wisDifference + MagicHittingChance;
                    if (nAttackTypea >= 1)
                    {
                        if (nAttackTypea > 70)
                            nAttackTypea = 70;
                    }
                    else
                    {
                        nAttackTypea = 1;
                    }

                    result = nAttackTypea + fxDef;
                    if (result >= 1)
                    {
                        if (result > 90)
                            result = 90;
                    }
                    else
                    {
                        result = 1;
                    }
                    return new Random().Next(1, 101) < result;
            }
            return true;
        }

        /// <summary>
        /// Calculates critical rate or possibility to make critical hit.
        /// Can be only more then 5 and less than 99.
        /// </summary>
        private int CriticalSuccessRate(IKillable target)
        {
            var result = Convert.ToInt32(CriticalHittingChance - (target.TotalLuc * 0.034000002));

            if (result < 5)
                result = 5;

            if (result > 99)
                result = 99;

            return result;
        }

        /// <summary>
        /// Possibility to hit enemy.
        /// </summary>
        public double PhysicalHittingChance { get => 1.0 * TotalDex / 2 + _skillPhysicalHittingChance; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public double PhysicalEvasionChance { get => 1.0 * TotalDex / 2 + _skillPhysicalEvasionChance; }

        /// <summary>
        /// Possibility to make critical hit.
        /// </summary>
        public double CriticalHittingChance { get => 0.2 * TotalLuc + _skillCriticalHittingChance; } // each 5 luck is 1% of critical.

        /// <summary>
        /// Possibility to hit enemy.
        /// </summary>
        public double MagicHittingChance { get => 1.0 * TotalWis / 2 + _skillMagicHittingChance; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public double MagicEvasionChance { get => 1.0 * TotalWis / 2 + _skillMagicEvasionChance; }

        #endregion
    }
}
