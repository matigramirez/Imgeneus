namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class RebirthConfiguration
    {
        /// <summary>
        /// Map id where a character must appear after it's death.
        /// </summary>
        public ushort MapId { get; set; }

        /// <summary>
        /// X coordinate.
        /// </summary>
        public float PosX { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float PosY { get; set; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public float PosZ { get; set; }
    }
}
