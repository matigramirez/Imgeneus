using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Microsoft.Extensions.Logging;
using System;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob : BaseKillable, IKiller, IDisposable
    {
        private readonly ILogger<Mob> _logger;
        private readonly DbMob _dbMob;

        public Mob(ILogger<Mob> logger, IDatabasePreloader databasePreloader, ushort mobId, bool shouldRebirth, MoveArea moveArea) : base(databasePreloader)
        {
            _logger = logger;
            _dbMob = databasePreloader.Mobs[mobId];

            CurrentHP = _dbMob.HP;
            Level = _dbMob.Level;
            AI = _dbMob.AI;
            ShouldRebirth = shouldRebirth;
            MoveArea = moveArea;

            if (ShouldRebirth)
            {
                _rebirthTimer.Interval = RespawnTimeInMilliseconds;
                _rebirthTimer.Elapsed += RebirthTimer_Elapsed;

                OnDead += MobRebirth_OnDead;
            }

            SetupAITimers();
            State = MobState.Idle;
        }

        /// <summary>
        /// Mob id from database.
        /// </summary>
        public ushort MobId => _dbMob.Id;

        /// <summary>
        /// Indicator, that shows if mob should rebirth after its' death.
        /// </summary>
        public bool ShouldRebirth { get; }

        #region Totel stats

        /// <inheritdoc />
        public override int TotalLuc => _dbMob.Luc;

        /// <inheritdoc />
        public override int TotalWis => _dbMob.Wis;

        /// <inheritdoc />
        public override int TotalDex => _dbMob.Dex;

        #endregion

        #region Max HP&SP&MP

        /// <inheritdoc />
        public override int MaxHP => _dbMob.HP;

        /// <inheritdoc />
        public override int MaxSP => _dbMob.SP;

        /// <inheritdoc />
        public override int MaxMP => _dbMob.MP;

        #endregion

        #region Defense & Resistance

        /// <inheritdoc />
        public override int Defense => _dbMob.Defense;

        /// <inheritdoc />
        public override int Resistance => _dbMob.Magic;

        #endregion

        #region Element

        /// <inheritdoc />
        public override Element DefenceElement => _dbMob.Element;

        /// <inheritdoc />
        public override Element AttackElement => _dbMob.Element;

        #endregion

        #region Hitting chance

        /// <inheritdoc />
        public override double PhysicalHittingChance => 1.0 * TotalDex / 2;

        /// <inheritdoc />
        public override double PhysicalEvasionChance => 1.0 * TotalDex / 2;

        /// <inheritdoc />
        public override double MagicHittingChance => 1.0 * TotalWis / 2;

        /// <inheritdoc />
        public override double MagicEvasionChance => 1.0 * TotalWis / 2;

        #endregion

        #region Stealth

        /// <inheritdoc />
        public override bool IsStealth { get; protected set; } = false;

        #endregion

        /// <summary>
        /// Creates mob clone.
        /// </summary>
        public Mob Clone()
        {
            return new Mob(_logger, _databasePreloader, MobId, ShouldRebirth, MoveArea);
        }

        public override void Dispose()
        {
            base.Dispose();
            ClearTimers();
        }

    }
}
