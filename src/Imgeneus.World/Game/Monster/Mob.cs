using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public class Mob : BaseKillable, IKiller
    {
        private readonly ILogger<Mob> _logger;
        private readonly DbMob _dbMob;

        public Mob(ILogger<Mob> logger, IDatabasePreloader databasePreloader, ushort mobId) : base(databasePreloader)
        {
            _logger = logger;
            _dbMob = databasePreloader.Mobs[mobId];

            CurrentHP = _dbMob.HP;
            Level = _dbMob.Level;
        }

        /// <summary>
        /// Mob id from database.
        /// </summary>
        public ushort MobId => _dbMob.Id;

        #region IKillable

        public override int MaxHP => _dbMob.HP;

        public override int MaxSP => _dbMob.SP;

        public override int MaxMP => _dbMob.MP;

        /// <inheritdoc />
        public override int TotalLuc => _dbMob.Luc;

        /// <inheritdoc />
        public override int TotalWis => _dbMob.Wis;

        /// <inheritdoc />
        public override int TotalDex => _dbMob.Dex;

        /// <inheritdoc />
        public override int Defense => _dbMob.Defense;

        /// <inheritdoc />
        public override int Resistance => _dbMob.Magic;

        /// <inheritdoc />
        public override Element DefenceElement => _dbMob.Element;

        /// <inheritdoc />
        public override Element AttackElement => _dbMob.Element;

        /// <inheritdoc />
        public override double PhysicalHittingChance => 1.0 * TotalDex / 2;

        /// <inheritdoc />
        public override double PhysicalEvasionChance => 1.0 * TotalDex / 2;

        /// <inheritdoc />
        public override double MagicHittingChance => 1.0 * TotalWis / 2;

        /// <inheritdoc />
        public override double MagicEvasionChance => 1.0 * TotalWis / 2;

        /// <inheritdoc />
        public override bool IsStealth { get; protected set; } = false;

        public override AttackSpeed AttackSpeed => AttackSpeed.Normal;

        public override int MoveSpeed { get; protected set; } = 2;

        #endregion

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

        #region Buffs

        protected override void BuffAdded(ActiveBuff buff)
        {
            // Implement if needed.
        }

        protected override void BuffRemoved(ActiveBuff buff)
        {
            // Implement if needed.
        }

        protected override void SendMoveAndAttackSpeed()
        {
            // Not implemented
        }

        protected override void SendAdditionalStats()
        {
            // Implement if needed.
        }

        #endregion
    }

    public enum MobMotion : byte
    {
        Walk = 0,
        Run = 1
    }
}
