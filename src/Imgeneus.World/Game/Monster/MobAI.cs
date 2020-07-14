using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Numerics;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob
    {
        #region AI

        /// <summary>
        /// Mob's ai type.
        /// </summary>
        public readonly MobAI AI;

        /// <summary>
        /// Mob's move area. It can not move farrer than this area.
        /// </summary>
        public readonly MoveArea MoveArea;

        private MobState _state = MobState.Idle;
        /// <summary>
        /// Current ai state.
        /// </summary>
        public MobState State
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;

                _logger.LogDebug($"Mob {Id} changed state to {_state}.");

                switch (_state)
                {
                    case MobState.Idle:
                        _idleTimer.Start();

                        // If this is combat mob start watching as soon as it's in idle state.
                        if (AI != MobAI.Peaceful && AI != MobAI.Peaceful2)
                            _watchTimer.Start();
                        break;

                    case MobState.Chase:
                        StartChasing();
                        break;

                    case MobState.BackToBirthPosition:
                        StopChasing();
                        // TODO: return to birth position.
                        break;

                    default:
                        _logger.LogWarning($"Not implemented mob state: {_state}.");
                        break;
                }
            }
        }

        /// <summary>
        /// Configures ai timers.
        /// </summary>
        private void SetupAITimers()
        {
            var idleTime = _dbMob.NormalTime <= 0 ? 4000 : _dbMob.NormalTime;
            _idleTimer.Interval = idleTime * 10;
            _idleTimer.AutoReset = false;
            _idleTimer.Elapsed += IdleTimer_Elapsed;

            _watchTimer.Interval = 1000; // 1 second
            _idleTimer.AutoReset = false;
            _watchTimer.Elapsed += WatchTimer_Elapsed;

            _chaseTimer.Interval = 500; // 0.5 second
            _chaseTimer.AutoReset = false;
            _chaseTimer.Elapsed += ChaseTimer_Elapsed;
        }

        /// <summary>
        /// Clears ai timers.
        /// </summary>
        private void ClearTimers()
        {
            _idleTimer.Elapsed -= IdleTimer_Elapsed;
        }

        #endregion

        #region Idle

        /// <summary>
        /// Mob walks around each N seconds, when he is in idle state.
        /// </summary>
        private readonly Timer _idleTimer = new Timer();

        private void IdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (State != MobState.Idle)
                return;

            GenerateRandomIdlePosition(_dbMob.NormalStep);

            _idleTimer.Start();
        }

        /// <summary>
        /// Generates new position for idle move.
        /// </summary>
        /// <param name="normalStep">idle step</param>
        private void GenerateRandomIdlePosition(byte normalStep)
        {
            float x1 = PosX - normalStep;
            if (x1 < MoveArea.X1)
                x1 = MoveArea.X1;
            float x2 = PosX + normalStep;
            if (x2 > MoveArea.X2)
                x2 = MoveArea.X2;

            float z1 = PosZ - normalStep;
            if (z1 < MoveArea.Z1)
                z1 = MoveArea.Z1;
            float z2 = PosZ + normalStep;
            if (z2 < MoveArea.Z2)
                z2 = MoveArea.Z2;

            PosX = new Random().NextFloat(x1, x2);
            PosZ = new Random().NextFloat(z1, z2);

            _logger.LogDebug($"Mob {Id} walks to new position x={PosX} y={PosY} z={PosZ}.");

            OnMove?.Invoke(this);
        }

        #endregion

        #region Move

        /// <inheritdoc />
        public override int MoveSpeed { get; protected set; } = 2;

        /// <summary>
        /// Event, that is fired, when mob moves.
        /// </summary>
        public event Action<Mob> OnMove;

        /// <summary>
        /// Describes if mob is "walking" or "running".
        /// </summary>
        public MobMotion MoveMotion
        {
            get
            {
                switch (State)
                {
                    case MobState.Idle:
                        return MobMotion.Walk;

                    case MobState.Chase:
                    case MobState.BackToBirthPosition:
                    case MobState.ReadyToAttack:
                        return MobMotion.Run;

                    default:
                        return MobMotion.Run;
                }
            }
        }

        #endregion

        #region Watch

        /// <summary>
        /// This timer triggers call to map in order to get list of players near by.
        /// </summary>
        private readonly Timer _watchTimer = new Timer();

        private void WatchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (State != MobState.Idle)
                return;

            Fraction playerFraction;
            switch (_dbMob.Fraction)
            {
                case MobFraction.Dark:
                    playerFraction = Fraction.Light;
                    break;

                case MobFraction.Light:
                    playerFraction = Fraction.Dark;
                    break;

                default:
                    playerFraction = Fraction.NotSelected;
                    break;
            }

            var players = Map.GetPlayers(PosX, PosZ, _dbMob.ChaseRange, playerFraction);

            // No players, keep watching.
            if (!players.Any())
            {
                _watchTimer.Start();
                return;
            }

            // There is some player in vision.
            Target = players.First();
            State = MobState.Chase;
        }

        #endregion

        #region Chase

        /// <summary>
        /// Chase timer triggers check if mob should follow user.
        /// </summary>
        private readonly Timer _chaseTimer = new Timer();

        /// <summary>
        /// Time, that is used to calculate delta time, which is used in speed calculation.
        /// </summary>
        private DateTime _lastChaseTime;

        /// <summary>
        /// Since when we sent the last update to players about mob position.
        /// </summary>
        private DateTime _lastMoveUpdate;

        /// <summary>
        /// Start chasing player.
        /// </summary>
        private void StartChasing()
        {
            _chaseTimer.Start();
            _lastChaseTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Stops chasing player.
        /// </summary>
        private void StopChasing()
        {
            _chaseTimer.Stop();
        }

        private void ChaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;
            var distanceToPlayer = MathExtensions.Distance(PosX, Target.PosX, PosZ, Target.PosZ);

            if (now > _nextAttackTime)
            {
                var attackId = RandomiseAttack(now);
                var useAttack1 = attackId == 1;
                var useAttack2 = attackId == 2;
                var useAttack3 = attackId == 3;

                if (useAttack1 && (distanceToPlayer <= _dbMob.AttackRange1 || _dbMob.AttackRange1 == 0))
                {
                    _logger.LogDebug($"Mob {Id} used attack 1.");
                    UseAttack(Target, _dbMob.AttackType1, _dbMob.Attack1, _dbMob.AttackAttrib1, _dbMob.AttackPlus1);
                    _nextAttackTime = now.AddMilliseconds(_dbMob.AttackTime1);
                    _lastAttack1Time = now;
                }

                if (useAttack2 && (distanceToPlayer <= _dbMob.AttackRange2 || _dbMob.AttackRange2 == 0))
                {
                    _logger.LogDebug($"Mob {Id} used attack 2.");
                    UseAttack(Target, _dbMob.AttackType2, _dbMob.Attack2, _dbMob.AttackAttrib2, _dbMob.AttackPlus2);
                    _nextAttackTime = now.AddMilliseconds(_dbMob.AttackTime2);
                    _lastAttack2Time = now;
                }

                if (useAttack3 && (distanceToPlayer <= _dbMob.AttackRange3 || _dbMob.AttackRange3 == 0))
                {
                    _logger.LogDebug($"Mob {Id} used attack 3.");
                    UseAttack(Target, _dbMob.AttackType3, _dbMob.Attack3, Element.None, _dbMob.AttackPlus3);
                    _nextAttackTime = now.AddMilliseconds(_dbMob.AttackTime3);
                    _lastAttack3Time = now;
                }
            }

            if (distanceToPlayer <= _dbMob.AttackRange1 || distanceToPlayer <= _dbMob.AttackRange2 || distanceToPlayer <= _dbMob.AttackRange3)
            {
                _lastChaseTime = now;
                _chaseTimer.Start();
                return;
            }

            var mobVector = new Vector2(PosX, PosZ);
            var playerVector = new Vector2(Target.PosX, Target.PosZ);

            var normalizedVector = Vector2.Normalize(playerVector - mobVector);
            var deltaTime = now.Subtract(_lastChaseTime);
            var temp = normalizedVector * (float)(_dbMob.ChaseStep * 1.0 / _dbMob.ChaseTime * deltaTime.TotalMilliseconds);
            PosX += float.IsNaN(temp.X) ? 0 : temp.X;
            PosZ += float.IsNaN(temp.Y) ? 0 : temp.Y;

            _lastChaseTime = now;

            // Send update to players, that mob position has changed.
            if (DateTime.UtcNow.Subtract(_lastMoveUpdate).TotalMilliseconds > 1000)
            {
                OnMove?.Invoke(this);
                _lastMoveUpdate = now;
            }

            _chaseTimer.Start();
        }

        /// <summary>
        /// Randomly selects the next attack.
        /// </summary>
        /// <param name="now">now time</param>
        /// <returns>attack type: 1, 2, 3 or 0, when can not attack</returns>
        private byte RandomiseAttack(DateTime now)
        {
            var useAttack1 = false;
            var useAttack2 = false;
            var useAttack3 = false;

            int chanceForAttack1 = 0;
            int chanceForAttack2 = 0;
            int chanceForAttack3 = 0;

            if (IsAttack1Enabled && IsAttack2Enabled && IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 60;
                else
                    chanceForAttack1 = 0;

                if (now.Subtract(_lastAttack2Time).TotalMilliseconds >= _dbMob.AttackTime2)
                    chanceForAttack2 = 85;
                else
                    chanceForAttack2 = 0;

                if (now.Subtract(_lastAttack3Time).TotalMilliseconds >= _dbMob.AttackTime3)
                    chanceForAttack3 = 100;
                else
                    chanceForAttack3 = 0;
            }
            else if (IsAttack1Enabled && IsAttack2Enabled && !IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 70;
                else
                    chanceForAttack1 = 0;

                if (now.Subtract(_lastAttack2Time).TotalMilliseconds >= _dbMob.AttackTime2)
                    chanceForAttack2 = 100;
                else
                    chanceForAttack2 = 0;

                chanceForAttack3 = 0;
            }
            else if (IsAttack1Enabled && !IsAttack2Enabled && !IsAttack3Enabled)
            {
                if (now.Subtract(_lastAttack1Time).TotalMilliseconds >= _dbMob.AttackTime1)
                    chanceForAttack1 = 100;
                else
                    chanceForAttack1 = 0;

                chanceForAttack2 = 0;
                chanceForAttack3 = 0;
            }
            if (!IsAttack1Enabled && !IsAttack2Enabled && !IsAttack3Enabled)
            {
                chanceForAttack1 = 0;
                chanceForAttack2 = 0;
                chanceForAttack3 = 0;
            }

            var random = new Random().Next(1, 100);
            if (random <= chanceForAttack1)
                useAttack1 = true;
            else if (random > chanceForAttack1 && random <= chanceForAttack2)
                useAttack2 = true;
            else if (random > chanceForAttack2 && random <= chanceForAttack3)
                useAttack3 = true;

            if (useAttack1)
                return 1;
            else if (useAttack2)
                return 2;
            else if (useAttack3)
                return 3;
            else
                return 0;
        }

        #endregion

        #region Attack

        /// <summary>
        /// Mob's target.
        /// </summary>
        public IKillable Target { get; private set; }

        /// <inheritdoc />
        public override AttackSpeed AttackSpeed => AttackSpeed.Normal;

        /// <summary>
        /// Event, that is fired, when mob attacks some user.
        /// </summary>
        public event Action<Mob, int, AttackResult> OnAttack;

        /// <summary>
        /// Time since the last attack.
        /// </summary>
        private DateTime _nextAttackTime;

        /// <summary>
        /// Clears target.
        /// </summary>
        public void ClearTarget()
        {
            State = MobState.BackToBirthPosition;
            Target = null;
        }

        /// <summary>
        /// Uses some attack.
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="attackType">type of attack</param>
        /// <param name="damage">normal damage</param>
        /// <param name="element">element</param>
        /// <param name="additionalDamage">plus damage</param>
        private void UseAttack(IKillable target, MobAttackType attackType, short damage, Element element, ushort additionalDamage)
        {
            // TODO: calculate damage.
            var res = new AttackResult(AttackSuccess.Normal, new Damage(10, 0, 0));

            _logger.LogDebug($"Mob {Id} deals damage to player {target.Id}: {res.Damage.HP} HP; {res.Damage.MP} MP; {res.Damage.SP} SP ");

            Target.DecreaseHP(res.Damage.HP, this);
            Target.CurrentMP -= res.Damage.MP;
            Target.CurrentSP -= res.Damage.SP;

            OnAttack?.Invoke(this, Target.Id, res);
        }

        #endregion

        #region Attack 1

        /// <summary>
        /// Time since the last attack 1.
        /// </summary>
        private DateTime _lastAttack1Time;

        /// <summary>
        /// Indicator of attack 1.
        /// </summary>
        private readonly bool IsAttack1Enabled;

        #endregion

        #region Attack 2

        /// <summary>
        /// Time since the last attack 2.
        /// </summary>
        private DateTime _lastAttack2Time;

        /// <summary>
        /// Indicator of attack 2.
        /// </summary>
        private readonly bool IsAttack2Enabled;

        #endregion

        #region Attack 3

        /// <summary>
        /// Time since the last attack 3.
        /// </summary>
        private DateTime _lastAttack3Time;

        /// <summary>
        /// Indicator of attack 3.
        /// </summary>
        private readonly bool IsAttack3Enabled;

        #endregion
    }
}
