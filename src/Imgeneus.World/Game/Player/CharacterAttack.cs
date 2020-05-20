using Imgeneus.Database.Constants;
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
        /// Stops casting.
        /// </summary>
        private void StopCasting()
        {
            _castTimer.Stop();
            _skillInCast = null;
            _targetInCast = null;
        }

        /// <summary>
        /// When time for casting has elapsed.
        /// </summary>
        private void CastTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UseSkill(_skillInCast, _targetInCast);
            StopCasting();
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
        private void Attack(int skillNumber, IKillable target)
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
        private void UseSkill(Skill skill, IKillable target)
        {
            SendAttackStart();

            if (!CanAttack(skill.Number, target))
                return;

            _nextAttackTime = DateTime.UtcNow.AddMilliseconds(NextAttackTime);

            CurrentMP -= skill.NeedMP;
            CurrentSP -= skill.NeedSP;
            SendUseSMMP(skill.NeedMP, skill.NeedSP);

            switch (skill.Type)
            {
                case TypeDetail.Buff:
                    UsedBuffSkill(skill, target);
                    break;
                default:
                    UsedAttackSkill(skill, target);
                    break;
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

            var result = CalculateDamage(Target);
            Target.DecreaseHP(result.Damage.HP, this);
            Target.CurrentSP -= result.Damage.SP;
            Target.CurrentMP -= result.Damage.MP;

            OnAutoAttack?.Invoke(this, result);
        }

        /// <summary>
        /// Calculates damage based on player stats and target stats.
        /// </summary>
        private AttackResult CalculateDamage(IKillable target, Skill skill = null)
        {
            if (skill is null)
            {
                Damage damage = new Damage(33, 0, 0);
                return new AttackResult(AttackSuccess.Normal, damage);
            }
            else
            {
                Damage damage = new Damage(100, 0, 0);
                return new AttackResult(AttackSuccess.Critical, damage);
            }
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

            if (skillNumber == 255 && target.IsDead)
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

            if (target.IsDead)
            {
                SendSkillWrongTarget(target, skill);
                return false;
            }

            if (CurrentMP < skill.NeedMP || CurrentSP < skill.NeedSP)
            {
                SendNotEnoughMPSP(Target, skill);
                return false;
            }

            return true;
        }

        #endregion
    }
}
