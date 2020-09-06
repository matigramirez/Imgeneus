using System.Collections.Generic;

namespace Imgeneus.World.Game.Zone.Obelisks
{
    public class MapObeliskConfigurations
    {
        public IEnumerable<MapObeliskConfiguration> Maps { get; set; }
    }

    public class MapObeliskConfiguration
    {
        public ushort MapId;

        public IEnumerable<ObeliskConfiguration> Obelisks { get; set; }
    }

    public class ObeliskConfiguration
    {
        /// <summary>
        /// Obelisk id.
        /// </summary>
        public int Id { get; set; }

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
        /// Obelisk mob id, when it's neutral obelisk.
        /// </summary>
        public ushort NeutralObeliskMobId { get; set; }

        /// <summary>
        /// Obelisk mob id, when it's light obelisk.
        /// </summary>
        public ushort LightObeliskMobId { get; set; }

        /// <summary>
        /// Obelisk mob id, when it's dark obelisk.
        /// </summary>
        public ushort DarkObeliskMobId { get; set; }

        /// <summary>
        /// Default obelisk, when server starts.
        /// </summary>
        public ObeliskCountry DefaultCountry { get; set; }

        /// <summary>
        /// Obelisk defenders.
        /// </summary>
        public IEnumerable<ObeliskMobConfiguration> Mobs { get; set; } = new List<ObeliskMobConfiguration>();
    }
}
