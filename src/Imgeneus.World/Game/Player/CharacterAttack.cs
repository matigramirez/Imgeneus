using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Monster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable
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
            SendAttackStart();

            if (!CanAttack(skill.Number, target))
                return;

            _nextAttackTime = DateTime.UtcNow.AddMilliseconds(NextAttackTime);

            CurrentMP -= skill.NeedMP;
            CurrentSP -= skill.NeedSP;
            SendUseSMMP(skill.NeedMP, skill.NeedSP);

            var targets = new List<IKillable>();
            switch (skill.TargetType)
            {
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

                if (!AttackSuccessRate(t, skill.TypeAttack, skill))
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

                    default:
                        throw new NotImplementedException("Not implemented skill type.");
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
                    return CalculateDamage(target, skill.TypeAttack, skill);

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
            Element attackElement = skill != null && skill.Element != Element.None ? skill.Element : AttackElement;
            var elementFactor = GetElementFactor(attackElement, target.DefenceElement);
            damage = damage * elementFactor;

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
            // Uncomment this code, if you want to always hit target.
            // return true;
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
        public override double PhysicalHittingChance { get => 1.0 * TotalDex / 2 + _skillPhysicalHittingChance; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public override double PhysicalEvasionChance { get => 1.0 * TotalDex / 2 + _skillPhysicalEvasionChance; }

        /// <summary>
        /// Possibility to make critical hit.
        /// </summary>
        public double CriticalHittingChance { get => 0.2 * TotalLuc + _skillCriticalHittingChance; } // each 5 luck is 1% of critical.

        /// <summary>
        /// Possibility to hit enemy.
        /// </summary>
        public override double MagicHittingChance { get => 1.0 * TotalWis / 2 + _skillMagicHittingChance; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public override double MagicEvasionChance { get => 1.0 * TotalWis / 2 + _skillMagicEvasionChance; }

        /// <summary>
        /// Calculates element multiplier based on attack and defence elements.
        /// </summary>
        public static double GetElementFactor(Element attackElement, Element defenceElement)
        {
            if (attackElement == defenceElement)
                return 1;

            if (attackElement != Element.None && defenceElement == Element.None)
            {
                if (attackElement == Element.Fire1 || attackElement == Element.Earth1 || attackElement == Element.Water1 || attackElement == Element.Wind1)
                    return 1.2;
                if (attackElement == Element.Fire2 || attackElement == Element.Earth2 || attackElement == Element.Water2 || attackElement == Element.Wind2)
                    return 1.3;
            }

            if (attackElement == Element.None && defenceElement != Element.None)
            {
                if (defenceElement == Element.Fire1 || defenceElement == Element.Earth1 || defenceElement == Element.Water1 || defenceElement == Element.Wind1)
                    return 0.8;
                if (defenceElement == Element.Fire2 || defenceElement == Element.Earth2 || defenceElement == Element.Water2 || defenceElement == Element.Wind2)
                    return 0.7;
            }

            if (attackElement == Element.Water1)
            {
                if (defenceElement == Element.Fire1)
                    return 1.4;
                if (defenceElement == Element.Fire2)
                    return 1.3;

                if (defenceElement == Element.Earth1)
                    return 0.5;
                if (defenceElement == Element.Earth2)
                    return 0.4;

                return 1; // wind or water
            }

            if (attackElement == Element.Fire1)
            {
                if (defenceElement == Element.Wind1)
                    return 1.4;
                if (defenceElement == Element.Wind2)
                    return 1.3;

                if (defenceElement == Element.Water1)
                    return 0.5;
                if (defenceElement == Element.Water2)
                    return 0.4;

                return 1; // earth or fire
            }

            if (attackElement == Element.Wind1)
            {
                if (defenceElement == Element.Earth1)
                    return 1.4;
                if (defenceElement == Element.Earth2)
                    return 1.3;

                if (defenceElement == Element.Fire1)
                    return 0.5;
                if (defenceElement == Element.Fire2)
                    return 0.4;

                return 1; // wind or water
            }

            if (attackElement == Element.Earth1)
            {
                if (defenceElement == Element.Water1)
                    return 1.4;
                if (defenceElement == Element.Water2)
                    return 1.3;

                if (defenceElement == Element.Wind1)
                    return 0.5;
                if (defenceElement == Element.Wind2)
                    return 0.4;

                return 1; // earth or fire
            }

            if (attackElement == Element.Water2)
            {
                if (defenceElement == Element.Fire1)
                    return 1.6;
                if (defenceElement == Element.Fire2)
                    return 1.4;

                if (defenceElement == Element.Earth1)
                    return 0.5;
                if (defenceElement == Element.Earth2)
                    return 0.5;

                return 1; // wind or water
            }

            if (attackElement == Element.Fire2)
            {
                if (defenceElement == Element.Wind1)
                    return 1.6;
                if (defenceElement == Element.Wind2)
                    return 1.4;

                if (defenceElement == Element.Water1)
                    return 0.5;
                if (defenceElement == Element.Water2)
                    return 0.5;

                return 1; // earth or fire
            }

            if (attackElement == Element.Wind2)
            {
                if (defenceElement == Element.Earth1)
                    return 1.6;
                if (defenceElement == Element.Earth2)
                    return 1.4;

                if (defenceElement == Element.Fire1)
                    return 0.5;
                if (defenceElement == Element.Fire2)
                    return 0.5;

                return 1; // wind or water
            }

            if (attackElement == Element.Earth2)
            {
                if (defenceElement == Element.Water1)
                    return 1.6;
                if (defenceElement == Element.Water2)
                    return 1.4;

                if (defenceElement == Element.Wind1)
                    return 0.5;
                if (defenceElement == Element.Wind2)
                    return 0.5;

                return 1; // earth or fire
            }

            return 1;
        }

        #endregion
    }
}
