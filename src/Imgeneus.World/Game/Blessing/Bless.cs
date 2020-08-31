using Imgeneus.Core.Common;
using Imgeneus.Database.Entities;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Blessing
{
    /// <summary>
    /// Goddess bless, gained by killing enemies and destroying altars.
    /// </summary>
    public class Bless : Singleton<Bless>
    {
        private object _syncObject = new object();
        public const int MAX_AMOUNT_VALUE = 12288;

        public Bless()
        {
            _fullBlessTimer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
            _fullBlessTimer.AutoReset = false;
            _fullBlessTimer.Elapsed += FullBlessTimer_Elapsed;
        }

        #region Definitions

        /// <summary>
        /// When bless amount >= <see cref="SP_MP_SIT"/> it recovers more sp, mp during break.
        /// </summary>
        public const int SP_MP_SIT = 150;

        /// <summary>
        /// When bless amount >= <see cref="HP_SIT"/> it adds sp, mp during break.
        /// </summary>
        public const int HP_SIT = 300;

        /// <summary>
        /// When bless amount >= <see cref="LINK_EXTRACT_LAPIS"/> it addes several % to link and extrack lapis.
        /// </summary>
        public const int LINK_EXTRACT_LAPIS = 1200;

        /// <summary>
        /// When bless amount >= <see cref="CAST_TIME_DISPOSABLE_ITEMS"/> it reduces cast time of dispoable items.
        /// </summary>
        public const int CAST_TIME_DISPOSABLE_ITEMS = 1350;

        /// <summary>
        ///  When bless amount >= <see cref="EXP_LOSS"/> it reduces exp loss, when player died.
        /// </summary>
        public const int EXP_LOSS = 1500;

        /// <summary>
        /// When bless amount >= <see cref="SHOOTING_MAGIC_DEFENCE"/> it increases shooting/magic defence power.
        /// </summary>
        public const int SHOOTING_MAGIC_DEFENCE = 2100;

        /// <summary>
        /// When bless amount >= <see cref="PHYSICAL_DEFENCE"/> it increases physical defence power.
        /// </summary>
        public const int PHYSICAL_DEFENCE = 2250;

        /// <summary>
        /// When bless amount >= <see cref="REPAIR_COST"/> it reduces repair costs.
        /// </summary>
        public const int REPAIR_COST = 2700;

        /// <summary>
        /// When bless amount >= <see cref="SP_MP_SIT"/> it recovers more hp, sp, mp during battle.
        /// </summary>
        public const int HP_SP_MP_BATTLE = 8400;

        /// <summary>
        /// When bless amount >= <see cref="MAX_HP_SP_MP"/> it increases max hp, mp, sp.
        /// </summary>
        public const int MAX_HP_SP_MP = 10200;

        /// <summary>
        /// When bless amount >= <see cref="STATS"/> it increases stats.
        /// </summary>
        public const int STATS = 12000;

        /// <summary>
        /// When bless amount >= <see cref="FULL_BLESS_BONUS"/> it increases critical hit rate, evasion of all attacks (shooting/magic/physical).
        /// </summary>
        public const int FULL_BLESS_BONUS = 12288;

        #endregion

        #region Amount

        private int _lightAmount;

        /// <summary>
        /// The event, that is fired, when the amount of light bless changes.
        /// </summary>
        public event Action<BlessArgs> OnLightBlessChanged;

        /// <summary>
        /// Bless amount of light fraction.
        /// </summary>
        public int LightAmount
        {
            get
            {
                return _lightAmount;
            }
            set
            {
                lock (_syncObject)
                {
                    if (IsFullBless)
                        return;

                    var oldValue = _lightAmount;

                    if (value > 0)
                        _lightAmount = value;
                    else
                        _lightAmount = 0;

                    if (_lightAmount >= MAX_AMOUNT_VALUE)
                    {
                        _lightAmount = MAX_AMOUNT_VALUE;
                        StartFullBless(Fraction.Light);
                    }

                    OnLightBlessChanged?.Invoke(new BlessArgs(oldValue, _lightAmount));
                }
            }
        }

        private int _darkAmount;

        /// <summary>
        /// The event, that is fired, when the amount of dark bless changes.
        /// </summary>
        public event Action<BlessArgs> OnDarkBlessChanged;

        /// <summary>
        /// Bless amount of dark fraction.
        /// </summary>
        public int DarkAmount
        {
            get
            {
                return _darkAmount;
            }
            set
            {
                lock (_syncObject)
                {
                    if (IsFullBless)
                        return;

                    var oldValue = _darkAmount;

                    if (value > 0)
                        _darkAmount = value;
                    else
                        _darkAmount = 0;

                    if (_darkAmount > MAX_AMOUNT_VALUE)
                    {
                        _darkAmount = MAX_AMOUNT_VALUE;
                        StartFullBless(Fraction.Dark);
                    }

                    OnDarkBlessChanged?.Invoke(new BlessArgs(oldValue, _darkAmount));
                }
            }
        }

        #endregion

        #region Full bless

        private readonly Timer _fullBlessTimer = new Timer();

        /// <summary>
        /// When full bless will end.
        /// </summary>
        public DateTime FullBlessingEnd { get; private set; }

        /// <summary>
        /// Remaining time till full bless ends.
        /// </summary>
        public uint RemainingTime
        {
            get
            {
                lock (_syncObject)
                {
                    if (IsFullBless)
                        return (uint)FullBlessingEnd.Subtract(DateTime.UtcNow).TotalMilliseconds;
                    else
                        return 0;
                }
            }
        }

        /// <summary>
        /// Indicates if now full bless is running.
        /// </summary>
        public bool IsFullBless { get; private set; }

        /// <summary>
        /// Starts full bless for some fraction.
        /// </summary>
        private void StartFullBless(Fraction fraction)
        {
            IsFullBless = true;

            if (fraction == Fraction.Light)
                DarkAmount = 0;

            if (fraction == Fraction.Dark)
                LightAmount = 0;

            FullBlessingEnd = DateTime.UtcNow.AddMinutes(10);
            _fullBlessTimer.Start();
        }

        private void FullBlessTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_syncObject)
            {
                DarkAmount = 0;
                LightAmount = 0;
                IsFullBless = false;
            }
        }

        #endregion
    }
}
