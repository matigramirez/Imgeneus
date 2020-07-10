using System;
using System.Timers;

namespace Imgeneus.World.Game.Monster
{
    public partial class Mob
    {
        public event Action<Mob> TimeToRebirth;

        public double RespawnTimeInMilliseconds
        {
            get
            {
                switch (_dbMob.AttackSpecial3)
                {
                    case Database.Constants.MobRespawnTime.Seconds_15:
                        return new TimeSpan(0, 0, 15).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Seconds_35:
                        return new TimeSpan(0, 0, 35).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_1:
                        return new TimeSpan(0, 1, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_3:
                        return new TimeSpan(0, 3, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_7:
                        return new TimeSpan(0, 7, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_10:
                        return new TimeSpan(0, 10, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_15:
                        return new TimeSpan(0, 15, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_30:
                        return new TimeSpan(0, 30, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Minutes_45:
                        return new TimeSpan(0, 45, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Hours_1:
                        return new TimeSpan(1, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Hours_12:
                        return new TimeSpan(12, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Hours_18:
                        return new TimeSpan(18, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Days_3:
                        return new TimeSpan(3, 0, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Days_5:
                        return new TimeSpan(5, 0, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.Days_7:
                        return new TimeSpan(7, 0, 0, 0).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.GRB:
                        return new TimeSpan(7, 0, 0, 0).Subtract(new TimeSpan(2, 0, 0)).TotalMilliseconds;

                    case Database.Constants.MobRespawnTime.TestEnv:
                        return 1;

                    default:
                        throw new NotImplementedException("Not implemented respawn time.");
                }
            }
        }

        private Timer _rebirthTimer = new Timer();

        /// <summary>
        /// Rebirth mob, when it's dead.
        /// </summary>
        private void MobRebirth_OnDead(IKillable sender, IKiller killer)
        {
            if (!ShouldRebirth)
                return;

            _rebirthTimer.Start();
        }

        /// <summary>
        /// When rebirth timer elapsed.
        /// </summary>
        private void RebirthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _rebirthTimer.Stop();
            TimeToRebirth?.Invoke(this);
        }
    }
}
