using Imgeneus.Database.Constants;
using System;
using System.Linq;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character : IKillable
    {
        #region Target

        /// <summary>
        /// Player fire this event to map in order to get target.
        /// </summary>
        public event Action<Character, int, TargetEntity> OnSeekForTarget;

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
        /// Event, that is fired, when user starts casting.
        /// </summary>
        public event Action<Character, IKillable, Skill> OnSkillCastStarted;

        /// <summary>
        /// Starts casting.
        /// </summary>
        /// <param name="skill">skill, that we are casting</param>
        private void StartCasting(Skill skill)
        {
            _skillInCast = skill;
            _castTimer.Interval = skill.CastTime * 250;
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
        }

        /// <summary>
        /// When time for casting has elapsed.
        /// </summary>
        private void CastTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UseSkill(_skillInCast);
            StopCasting();

            if (_nextSkillNumber != 0 && !_attackTimer.Enabled)
                UseSkill(_nextSkillNumber);
        }

        #endregion

        #region Damage calculation

        /// <summary>
        /// The time since the last attack. Needed for attack calculations.
        /// </summary>
        private Timer _attackTimer = new Timer();

        /// <summary>
        /// This sync object is used for attack timer calculations.
        /// </summary>
        private object syncObject = new object();

        private int _nextSkillNumber;
        /// <summary>
        /// Client sends next skill number, that he/she wants to use.
        /// We should save this number and use skill based on elapsed time.
        /// </summary>
        public int NextSkillNumber
        {
            get => _nextSkillNumber;
            set
            {
                lock (syncObject)
                {
                    // First, check if target is not yet dead.
                    if (Target.IsDead)
                    {
                        if (value == 255)
                            SendAutoAttackWrongTarget(Target);
                        else
                            SendSkillWrongTarget(Target, Skills.First(s => s.Number == value));
                        return;
                    }

                    // If timer is not running, this means player just started attacking.
                    if (!_attackTimer.Enabled && !_castTimer.Enabled)
                    {
                        _attackTimer.Start();
                        _nextSkillNumber = value;
                        UseSkill(_nextSkillNumber);
                    }
                    // Set the next skill number and wait until timer calls it.
                    else
                    {
                        _nextSkillNumber = value;
                    }
                }
            }
        }

        /// <summary>
        /// Uses skill, based on its' number.
        /// </summary>
        private void UseSkill(int skillNumber)
        {
            if (skillNumber == 255)
                AutoAttack();
            else
            {
                var skill = Skills.First(s => s.Number == skillNumber);

                if (skill.CastTime == 0)
                    UseSkill(skill);
                else
                    StartCasting(skill);
            }
        }

        /// <summary>
        /// Make character use skill.
        /// </summary>
        private void UseSkill(Skill skill)
        {
            _nextSkillNumber = 0;

            if (CurrentMP < skill.NeedMP)
            {
                // TODO: send not enough MP.
                return;
            }

            if (CurrentSP < skill.NeedSP)
            {
                // TODO: send not enough SP.
                return;
            }

            CurrentMP -= skill.NeedMP;
            CurrentSP -= skill.NeedSP;
            SendUseSMMP(skill.NeedMP, skill.NeedSP);

            SendAttackStart();
            switch (skill.Type)
            {
                case TypeDetail.Buff:
                    UsedBuffSkill(skill);
                    break;
                default:
                    UsedAttackSkill(skill);
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
            if (Target.IsDead)
            {
                return;
            }

            SendAttackStart();
            _nextSkillNumber = 0;

            var result = CalculateDamage(Target);
            Target.DecreaseHP(result.Damage.HP, this);
            Target.CurrentSP -= result.Damage.SP;
            Target.CurrentMP -= result.Damage.MP;

            OnAutoAttack?.Invoke(this, result);
        }

        /// <summary>
        /// When time has elapsed, we can use another skill.
        /// </summary>
        private void AttackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If there is any pending skill.
            if (_nextSkillNumber != 0 && !_castTimer.Enabled)
                UseSkill(_nextSkillNumber);
            else // No pending skill, stop timer.
                _attackTimer.Stop();
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

        #endregion
    }
}
