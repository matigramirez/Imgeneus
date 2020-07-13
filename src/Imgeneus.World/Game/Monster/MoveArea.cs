namespace Imgeneus.World.Game.Monster
{
    /// <summary>
    /// Mob move area.
    /// </summary>
    public struct MoveArea
    {
        /// <summary>
        /// Left most X.
        /// </summary>
        public float X1;

        /// <summary>
        /// Right most X.
        /// </summary>
        public float X2;

        /// <summary>
        /// Left most Y.
        /// </summary>
        public float Y1;

        /// <summary>
        /// Right most Y.
        /// </summary>
        public float Y2;

        /// <summary>
        /// Left most Z.
        /// </summary>
        public float Z1;

        /// <summary>
        /// Right most Z.
        /// </summary>
        public float Z2;

        public MoveArea(float x1, float x2, float y1, float y2, float z1, float z2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            Z1 = z1;
            Z2 = z2;
        }
    }
}
