using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.World.Game.Blessing;
using Imgeneus.World.Game.Monster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Imgeneus.World.Game.Zone.Obelisks
{
    public class Obelisk : IMapMember
    {
        private readonly ObeliskConfiguration _config;
        private readonly IDatabasePreloader _databasePreloader;

        public Obelisk(ObeliskConfiguration config, IDatabasePreloader databasePreloader, Map map)
        {
            _config = config;
            _databasePreloader = databasePreloader;
            Map = map;

            Init();
        }

        #region Map member

        public int Id => _config.Id;

        public float PosX => _config.PosX;

        public float PosY => _config.PosY;

        public float PosZ => _config.PosZ;

        public ushort Angle => 0;

        public Map Map { get; private set; }

        public int CellId { get; set; }

        public int OldCellId { get; set; }

        #endregion

        private void Init()
        {
            ObeliskCountry = _config.DefaultCountry;

            ushort mobId = 0;
            if (ObeliskCountry == ObeliskCountry.None)
                mobId = _config.NeutralObeliskMobId;
            else if (ObeliskCountry == ObeliskCountry.Light)
                mobId = _config.LightObeliskMobId;
            else if (ObeliskCountry == ObeliskCountry.Dark)
                mobId = _config.DarkObeliskMobId;

            ObeliskAI = new Mob(DependencyContainer.Instance.Resolve<ILogger<Mob>>(),
                                _databasePreloader,
                                mobId,
                                false,
                                new MoveArea(PosX, PosX, PosY, PosY, PosZ, PosZ),
                                Map);

            Map.AddMob(ObeliskAI);
            ObeliskAI.OnDead += ObeliskAI_OnDead;

            // TODO: init guards.

            _rebirthTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            _rebirthTimer.AutoReset = false;
            _rebirthTimer.Elapsed += RebirthTimer_Elapsed;
        }

        #region Obelisk

        private Timer _rebirthTimer = new Timer();

        /// <summary>
        /// To whom obelisk belongs to? None, light or dark.
        /// </summary>
        public ObeliskCountry ObeliskCountry { get; private set; }

        /// <summary>
        /// Event is fired, when another fraction has broken obelisk.
        /// </summary>
        public event Action<Obelisk> OnObeliskBroken;

        /// <summary>
        /// Obelisk itself.
        /// </summary>
        public Mob ObeliskAI { get; private set; }

        private void ObeliskAI_OnDead(IKillable sender, IKiller killer)
        {
            ObeliskAI.OnDead -= ObeliskAI_OnDead;

            ObeliskCountry = killer.Country == Fraction.Light ? ObeliskCountry.Light : ObeliskCountry.Dark;
            OnObeliskBroken?.Invoke(this);

            if (ObeliskCountry == ObeliskCountry.Light)
            {
                Bless.Instance.LightAmount += 1000; // TODO: maybe different value for different obelisks?
                Bless.Instance.DarkAmount -= 500;
            }
            else
            {
                Bless.Instance.DarkAmount += 1000;
                Bless.Instance.LightAmount -= 500;
            }

            // TODO: clear guards.

            _rebirthTimer.Start();
        }

        private void RebirthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Bless.Instance.IsFullBless)
                return;

            if (ObeliskCountry == ObeliskCountry.Light)
            {
                // Init new ai.
                ObeliskAI = new Mob(DependencyContainer.Instance.Resolve<ILogger<Mob>>(),
                                    _databasePreloader,
                                    _config.LightObeliskMobId,
                                    false,
                                    new MoveArea(PosX, PosX, PosY, PosY, PosZ, PosZ),
                                    Map);
                ObeliskAI.OnDead += ObeliskAI_OnDead;
                Map.AddMob(ObeliskAI);

                // TODO: init new guards.
            }
            else
            {
                // Init new ai.
                ObeliskAI = new Mob(DependencyContainer.Instance.Resolve<ILogger<Mob>>(),
                                    _databasePreloader,
                                    _config.DarkObeliskMobId,
                                    false,
                                    new MoveArea(PosX, PosX, PosY, PosY, PosZ, PosZ),
                                    Map);
                ObeliskAI.OnDead += ObeliskAI_OnDead;
                Map.AddMob(ObeliskAI);

                // TODO: init new guards.
            }
        }


        #endregion

        #region Obelisk guards

        /// <summary>
        /// Obelisk defenders.
        /// </summary>
        public IEnumerable<Mob> Guards { get; private set; } = new List<Mob>();

        #endregion
    }
}
