using Imgeneus.Core.Extensions;
using Imgeneus.Database.Constants;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using System;
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

                switch (_state)
                {
                    case MobState.Idle:
                        _idleTimer.Start();
                        MoveMotion = MobMotion.Walk;
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
            _idleTimer.Elapsed += IdleTimer_Elapsed;
        }

        /// <summary>
        /// Clears ai timers.
        /// </summary>
        private void ClearTimers()
        {
            _idleTimer.Elapsed -= IdleTimer_Elapsed;
        }

        #endregion

        #region Idle state

        /// <summary>
        /// Mob walks around each N seconds, when he is in idle state.
        /// </summary>
        private Timer _idleTimer = new Timer();

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
        public MobMotion MoveMotion { get; private set; }

        #endregion

        #region Attack

        public int TargetId;

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
            TargetId = targetId;
            var timer = new Timer();
            timer.Interval = 3000; // 3 seconds.
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                OnAttack?.Invoke(this, TargetId);
            };
            timer.Start();
        }

        #endregion
    }
}
