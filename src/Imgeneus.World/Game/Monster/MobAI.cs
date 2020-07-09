using Imgeneus.World.Game.Player;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob
    {
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
        public MobMotion MoveMotion;

        /// <summary>
        /// TODO: remove me! This is only for move emulation.
        /// </summary>
        public void EmulateMovement()
        {
            var timer = new Timer();
            timer.Interval = 3000; // 3 seconds.
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                PosX += 5;
                PosZ += 5;
                OnMove?.Invoke(this);
            };
            timer.Start();

        }

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
