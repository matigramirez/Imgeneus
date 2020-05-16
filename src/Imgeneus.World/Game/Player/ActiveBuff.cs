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
        private Timer _resetTimer = new Timer();

        private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _resetTimer.Elapsed -= ResetTimer_Elapsed;
            OnReset?.Invoke(this);
        }

        /// <summary>
        /// Event, that is fired, when it's time to remove buff.
        /// </summary>
        public event Action<ActiveBuff> OnReset;

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
