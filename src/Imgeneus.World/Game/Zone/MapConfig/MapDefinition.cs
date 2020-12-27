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
        /// How long it's raining, snowing. In seconds.
        /// </summary>
        public byte WeatherDuration { get; set; }

        /// <summary>
        /// How long it's "good" weather (when it's not showing or raining). In seconds.
        /// </summary>
        public byte NoneWeatherDuration { get; set; }

        /// <summary>
        /// For party, guild etc.
        /// </summary>
        public CreateType CreateType { get; set; }

        /// <summary>
        /// Minimum members needed to enter map.
        /// </summary>
        public int MinMembersCount { get; set; }

        /// <summary>
        /// Maximum members needed to enter map.
        /// </summary>
        public int MaxMembersCount { get; set; }

        /// <summary>
        /// Checks if map is open at that time.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                // TODO: implement maps, that are open based on date time.
                return true;
            }
        }
    }
}
