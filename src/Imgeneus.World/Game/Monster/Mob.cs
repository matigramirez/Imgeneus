using Imgeneus.Database.Constants;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Microsoft.Extensions.Logging;
using System;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob : BaseKillable, IKiller, IDisposable
    {
        private readonly ILogger<Mob> _logger;
        private readonly DbMob _dbMob;

        public Mob(ILogger<Mob> logger,
                   IDatabasePreloader databasePreloader,
                   ushort mobId,
                   bool shouldRebirth,
                   MoveArea moveArea,
                   Map map) : base(databasePreloader)
        {
            _logger = logger;
            _dbMob = databasePreloader.Mobs[mobId];

            CurrentHP = _dbMob.HP;
            Level = _dbMob.Level;
            AI = _dbMob.AI;
            ShouldRebirth = shouldRebirth;
            MoveArea = moveArea;
            Map = map;
            IsAttack1Enabled = _dbMob.AttackOk1 != 0;
            IsAttack2Enabled = _dbMob.AttackOk2 != 0;
            IsAttack3Enabled = _dbMob.AttackOk3 != 0;

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

        /// <summary>
        /// Map, where mob was created.
        /// </summary>
        private readonly Map Map;

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

        /// <summary>
        /// Creates mob clone.
        /// </summary>
        public Mob Clone()
        {
            return new Mob(_logger, _databasePreloader, MobId, ShouldRebirth, MoveArea, Map);
        }

        public override void Dispose()
        {
            base.Dispose();
            ClearTimers();
        }
    }
}
