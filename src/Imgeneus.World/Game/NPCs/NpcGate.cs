namespace Imgeneus.World.Game.NPCs
{
    public class NpcGate
    {
        /// <summary>
        /// Map id.
        /// </summary>
        public ushort MapId { get; set; }

        /// <summary>
        /// X coordinate.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Z coordinate.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// User friendly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much does it cost to teleport.
        /// </summary>
        public int Cost { get; set; }
    }
}
