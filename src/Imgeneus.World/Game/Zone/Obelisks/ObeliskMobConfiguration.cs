namespace Imgeneus.World.Game.Zone.Obelisks
{
    public class ObeliskMobConfiguration
    {
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

        /// <summary>
        /// Mob vision?
        /// </summary>
        public byte Radius { get; set; }

        /// <summary>
        /// Mob id, when obelisk is neutral.
        /// </summary>
        public ushort NeutralMobId { get; set; }

        /// <summary>
        /// Mob id, when obelisk is light.
        /// </summary>
        public ushort LightMobId { get; set; }

        /// <summary>
        /// Mob id, when obelisk is dark.
        /// </summary>
        public ushort DarkMobId { get; set; }

        /// <summary>
        /// Number of mobs.
        /// </summary>
        public byte Count { get; set; }
    }
}