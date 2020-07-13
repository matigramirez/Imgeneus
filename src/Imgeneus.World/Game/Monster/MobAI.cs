using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
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
            _idleTimer.Interval = _dbMob.NormalTime * 10;
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

            var players = Map.GetPlayers(PosX, PosZ, _dbMob.ChaseRange);

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

        private void StopChasing()
        {
            _chaseTimer.Stop();
        }

        private void ChaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;

            if (MathExtensions.Distance(PosX, Target.PosX, PosZ, Target.PosZ) <= _dbMob.AttackRange1)
            {
                // TODO: use attack 1.
                _logger.LogDebug("Got close to target player");
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
        public event Action<Mob, int> OnAttack;

        /// <summary>
        /// TODO: remove me! This is only for attack emulation.
        /// </summary>
        public void EmulateAttack(int targetId)
        {
            var timer = new Timer();
            timer.Interval = 3000; // 3 seconds.
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                OnAttack?.Invoke(this, Target.Id);
            };
            timer.Start();
        }

        /// <summary>
        /// Clears target.
        /// </summary>
        public void ClearTarget()
        {
            State = MobState.BackToBirthPosition;
            Target = null;
        }

        #endregion
    }
}
