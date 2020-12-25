using Imgeneus.World.Game.Player;
using System;
using System.Timers;

namespace Imgeneus.World.Game.Zone
{
    /// <summary>
    /// Wrapper for inventory item, when it's added to map.
    /// </summary>
    public class MapItem : IMapMember
    {
        public int Id { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public ushort Angle { get; set; }

        public Map Map { get; set; }

        public int CellId { get; set; }

        public int OldCellId { get; set; }

        /// <summary>
        /// Thrown item.
        /// </summary>
        public Item Item { get; private set; }

        #region Owner

        private Timer _ownerClearTimer = new Timer();
        private Character _owner;
        /// <summary>
        /// Item owner, when item is dropped in the map.
        /// </summary>
        public Character Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
                if (_owner is null)
                    _ownerClearTimer.Stop();
                else
                    _ownerClearTimer.Start();

            }
        }

        private void OwnerClearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _owner = null;
            _removeTimer.Start();
        }

        #endregion

        #region Remove item timer

        private Timer _removeTimer = new Timer();

        private void RemoveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnRemove?.Invoke(this);
        }

        /// <summary>
        /// Event, that is fired, when it's time to remove item from map.
        /// </summary>
        public event Action<MapItem> OnRemove;

        /// <summary>
        /// Stops remove timer.
        /// </summary>
        public void StopRemoveTimer()
        {
            _removeTimer.Stop();
        }

        #endregion

        public MapItem(Item item, Character owner, float x, float y, float z)
        {
            Item = item;
            Owner = owner;
            PosX = x;
            PosY = y;
            PosZ = z;

            _ownerClearTimer.Interval = 20000; // 20 seconds
            _ownerClearTimer.AutoReset = false;
            _ownerClearTimer.Elapsed += OwnerClearTimer_Elapsed;

            _removeTimer.Interval = 60000; // 60 seconds
            _removeTimer.AutoReset = false;
            _removeTimer.Elapsed += RemoveTimer_Elapsed;
        }
    }
}
