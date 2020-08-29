namespace Imgeneus.World.Game.Zone
{
    public interface IMapMember
    {
        /// <summary>
        /// Unique id in this map.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// X coordinate.
        /// </summary>
        public float PosX { get; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float PosY { get; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public float PosZ { get; }

        /// <summary>
        /// Angle.
        /// </summary>
        public ushort Angle { get; }

        /// <summary>
        /// Current map.
        /// </summary>
        public Map Map { get; }

        /// <summary>
        /// For performance improvement each map member stores cell id to which it belongs to.
        /// </summary>
        public int CellId { get; set; }
    }
}
