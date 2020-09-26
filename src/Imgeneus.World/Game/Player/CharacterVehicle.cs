using System;
using System.Timers;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        private bool _isSummmoningVehicle;

        private readonly Timer _summonVehicleTimer = new Timer()
        {
            AutoReset = false
        };

        /// <summary>
        /// Is player currently summoning vehicle?
        /// </summary>
        public bool IsSummmoningVehicle
        {
            get => _isSummmoningVehicle;
            private set
            {
                _isSummmoningVehicle = value;
                if (_isSummmoningVehicle)
                {
                    _summonVehicleTimer.Interval = Mount.AttackSpeed > 0 ? Mount.AttackSpeed * 1000 : Mount.AttackSpeed + 1000;
                    _summonVehicleTimer.Start();
                    OnStartSummonVehicle?.Invoke(this);
                }
                else
                {
                    _summonVehicleTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Event, that is fired, when the player starts summoning mount. 
        /// </summary>
        public event Action<Character> OnStartSummonVehicle;

        private bool _isOnVehicle;
        /// <summary>
        /// Indicator if character is on mount now.
        /// </summary>
        public bool IsOnVehicle
        {
            get => _isOnVehicle;
            private set
            {
                if (_isOnVehicle == value)
                    return;

                _isOnVehicle = value;

                OnShapeChange?.Invoke(this);
                InvokeAttackOrMoveChanged();
            }
        }

        /// <summary>
        /// Tries to summon vehicle(mount).
        /// </summary>
        public void CallVehicle()
        {
            if (Mount is null || IsStealth)
                return;

            IsSummmoningVehicle = true;
        }

        /// <summary>
        /// Unsummons vehicle(mount).
        /// </summary>
        public void RemoveVehicle()
        {
            IsOnVehicle = false;
        }

        /// <summary>
        /// Stops summon timer.
        /// </summary>
        public void CancelVehicleSummon()
        {
            IsSummmoningVehicle = false;
        }

        private void SummonVehicleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendUseVehicle(true, true);
            IsOnVehicle = true;
        }
    }
}
