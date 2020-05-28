using Imgeneus.Database.Entities;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public class ActiveBuff
    {
        private static uint Counter = 1;

        public uint Id { get; private set; }

        private object SyncObj = new object();

        public int CountDownInSeconds { get => (int)ResetTime.Subtract(DateTime.UtcNow).TotalSeconds; }

        public ushort SkillId { get; set; }

        public byte SkillLevel { get; set; }

        public ActiveBuff()
        {
            lock (SyncObj)
            {
                Id = Counter++;
            }

            _resetTimer.Elapsed += ResetTimer_Elapsed;
            _periodicalHealTimer.Elapsed += PeriodicalHealTimer_Elapsed;
        }

        #region Buff reset

        private DateTime _resetTime;
        /// <summary>
        /// Time, when buff is going to turn off.
        /// </summary>
        public DateTime ResetTime
        {
            get => _resetTime;
            set
            {
                _resetTime = value;

                // Set up timer.
                _resetTimer.Stop();
                _resetTimer.Interval = _resetTime.Subtract(DateTime.UtcNow).TotalMilliseconds;
                _resetTimer.Start();
            }
        }

        /// <summary>
        /// Timer, that is called when it's time to remove buff.
        /// </summary>
        private readonly Timer _resetTimer = new Timer();

        private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _resetTimer.Elapsed -= ResetTimer_Elapsed;
            _resetTimer.Stop();
            _periodicalHealTimer.Elapsed -= PeriodicalHealTimer_Elapsed;
            _periodicalHealTimer.Stop();

            OnReset?.Invoke(this);
        }

        /// <summary>
        /// Event, that is fired, when it's time to remove buff.
        /// </summary>
        public event Action<ActiveBuff> OnReset;

        #endregion

        #region Periodical Heal

        /// <summary>
        /// Timer, that is called when it's time to make periodical heal (every 3 seconds).
        /// </summary>
        private readonly Timer _periodicalHealTimer = new Timer(3000);

        /// <summary>
        /// Event, that is fired, when it's time to make periodical heal.
        /// </summary>
        public event Action<ActiveBuff, AttackResult> OnPeriodicalHeal;

        public ushort TimeHealHP;

        public ushort TimeHealSP;

        public ushort TimeHealMP;

        private void PeriodicalHealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPeriodicalHeal?.Invoke(this, new AttackResult(AttackSuccess.Normal, new Damage(TimeHealHP, TimeHealSP, TimeHealMP)));
        }

        /// <summary>
        /// Starts periodical healing.
        /// </summary>
        public void StartPeriodicalHeal()
        {
            _periodicalHealTimer.Start();
        }

        #endregion

        public static ActiveBuff FromDbCharacterActiveBuff(DbCharacterActiveBuff buff)
        {
            return new ActiveBuff()
            {
                ResetTime = buff.ResetTime,
                SkillId = buff.Skill.SkillId,
                SkillLevel = buff.Skill.SkillLevel
            };
        }
    }
}
