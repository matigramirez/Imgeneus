using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class MapDefinitions
    {
        /// <summary>
        /// List of maps.
        /// </summary>
        [JsonPropertyName("maps")]
        public List<MapDefinition> Maps { get; set; }
    }

    public class MapDefinition
    {
        /// <summary>
        /// Map Id.
        /// </summary>
        [JsonPropertyName("MapId")]
        public ushort Id { get; set; }

        /// <summary>
        /// Field of dungeon.
        /// </summary>
        public MapType MapType { get; set; }

        /// <summary>
        /// None, rainy, snowy.
        /// </summary>
        public WeatherState WeatherState { get; set; }

        /// <summary>
        /// Light, normal or hard.
        /// </summary>
        public WeatherPower WeatherPower { get; set; }

        /// <summary>
        /// How often it's raining, snowing. In minutes.
        /// </summary>
        public byte WeatherDelay { get; set; }

        /// <summary>
        /// For party, guild etc.
        /// </summary>
        public CreateType CreateType { get; set; }
    }
}
