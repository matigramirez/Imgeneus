namespace Imgeneus.World.Game
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
    }
}
