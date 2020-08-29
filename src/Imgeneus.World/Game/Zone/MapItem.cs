using Imgeneus.World.Game.Player;
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
        }

        #endregion

        public MapItem(Item item, Character owner, float x, float y, float z)
        {
            Item = item;
            Owner = owner;
            PosX = x;
            PosY = y;
            PosZ = z;

            _ownerClearTimer.Interval = 7000; // 7 seconds
            _ownerClearTimer.AutoReset = false;
            _ownerClearTimer.Elapsed += OwnerClearTimer_Elapsed;
        }
    }
}
