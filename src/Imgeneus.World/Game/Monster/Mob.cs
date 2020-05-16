using Imgeneus.Database.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public class Mob : IKillable, IKiller
    {
        private readonly ILogger<Mob> _logger;

        public Mob(ILogger<Mob> logger)
        {
            _logger = logger;
        }

        private int _id;

        /// <inheritdoc />
        public int Id
        {
            get => _id;
            set
            {
                if (_id == 0)
                {
                    _id = value;
                }
                else
                {
                    throw new ArgumentException("Mob id can not be set twice.");
                }

            }
        }

        #region IKillable

        /// <inheritdoc />
        public IKiller MyKiller { get; private set; }

        /// <inheritdoc />
        public void DecreaseHP(int hp, IKiller damageMaker)
        {
            CurrentHP -= hp;
            MyKiller = damageMaker;

            if (CurrentHP < 0)
                OnDead?.Invoke(this, MyKiller);
        }

        /// <inheritdoc />
        public int CurrentHP { get; set; }

        /// <inheritdoc />
        public event Action<IKillable, IKiller> OnDead;

        /// <inheritdoc />
        public int CurrentSP { get; set; }

        /// <inheritdoc />
        public int CurrentMP { get; set; }

        /// <inheritdoc />
        public bool IsDead => false;

        #endregion

        /// <summary>
        /// Mob id from database.
        /// </summary>
        public ushort MobId;

        /// <summary>
        /// Current x position.
        /// </summary>
        public float PosX;

        /// <summary>
        /// Current y position.
        /// </summary>
        public float PosY;

        /// <summary>
        /// Current z position.
        /// </summary>
        public float PosZ;

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

        public int TargetId;

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

        public event Action<Mob> OnMove;

        /// <summary>
        /// Event, that is fired, when mob attacks some user.
        /// </summary>
        public event Action<Mob, int> OnAttack;

        public static Mob FromDbMob(DbMob mob, ILogger<Mob> logger)
        {
            return new Mob(logger)
            {
                MobId = mob.Id,
                CurrentHP = mob.HP
            };
        }
    }

    public enum MobMotion : byte
    {
        Walk = 0,
        Run = 1
    }
}
