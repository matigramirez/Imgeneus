using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class MapConfiguration
    {
        /// <summary>
        /// Map size.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// Minimum cell size.
        /// </summary>
        [JsonPropertyName("cell_size")]
        public int CellSize { get; set; }

        /// <summary>
        /// Mob areas.
        /// </summary>
        [JsonPropertyName("mob_areas")]
        public List<MobAreaConfiguration> MobAreas { get; set; } = new List<MobAreaConfiguration>();

        /// <summary>
        /// NPCs.
        /// </summary>
        [JsonPropertyName("npcs")]
        public List<NpcConfiguration> NPCs { get; set; } = new List<NpcConfiguration>();

        /// <summary>
        /// Portals.
        /// </summary>
        [JsonPropertyName("portals")]
        public List<PortalConfiguration> Portals { get; set; } = new List<PortalConfiguration>();

        /// <summary>
        /// Spawns.
        /// </summary>
        [JsonPropertyName("spawns")]
        public List<SpawnConfiguration> Spawns { get; set; } = new List<SpawnConfiguration>();

        /// <summary>
        /// Ladders.
        /// </summary>
        [JsonPropertyName("ladders")]
        public List<LadderConfiguration> Ladders { get; set; } = new List<LadderConfiguration>();

        /// <summary>
        /// Named areas.
        /// </summary>
        [JsonPropertyName("named_areas")]
        public List<NamedAreaConfiguration> NamedAreas { get; set; } = new List<NamedAreaConfiguration>();
    }
}
