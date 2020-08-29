using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class MapConfiguration
    {
        /// <summary>
        /// Map id.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Map size.
        /// </summary>
        [JsonPropertyName("size")]
        public int Size { get; set; }

        /// <summary>
        /// Mob areas.
        /// </summary>
        [JsonPropertyName("mob_areas")]
        public List<MobAreaConfiguration> MobAreas { get; set; }

        /// <summary>
        /// NPCs.
        /// </summary>
        [JsonPropertyName("npcs")]
        public List<NpcConfiguration> NPCs { get; set; }

        /// <summary>
        /// Portals.
        /// </summary>
        [JsonPropertyName("portals")]
        public List<PortalConfiguration> Portals { get; set; }

        /// <summary>
        /// Spawns.
        /// </summary>
        [JsonPropertyName("spawns")]
        public List<SpawnConfiguration> Spawns { get; set; }

        /// <summary>
        /// Ladders.
        /// </summary>
        [JsonPropertyName("ladders")]
        public List<LadderConfiguration> Ladders { get; set; }

        /// <summary>
        /// Named areas.
        /// </summary>
        [JsonPropertyName("named_areas")]
        public List<NamedAreaConfiguration> NamedAreas { get; set; }
    }
}
