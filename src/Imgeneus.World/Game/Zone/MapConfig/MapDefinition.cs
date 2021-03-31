using NCrontab;
using System;
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
        public bool IsOpen(DateTime now)
        {
            var startNext = NextOpenDate(now);
            var endNext = NextCloseDate(now);

            return endNext < startNext;
        }

        /// <summary>
        /// Map's open time in NCrontab format. More info here: https://github.com/atifaziz/NCrontab
        /// </summary>
        public string OpenTime { get; set; }

        /// <summary>
        /// Generates the next open date.
        /// </summary>
        public DateTime NextOpenDate(DateTime now)
        {
            var start = CrontabSchedule.Parse(OpenTime);
            return start.GetNextOccurrence(now);
        }

        /// <summary>
        /// Map's open time in NCrontab format. More info here: https://github.com/atifaziz/NCrontab
        /// </summary>
        public string CloseTime { get; set; }

        /// <summary>
        /// Generates the next close date.
        /// </summary>
        public DateTime NextCloseDate(DateTime now)
        {
            var end = CrontabSchedule.Parse(CloseTime);
            return end.GetNextOccurrence(now);
        }

        /// <summary>
        /// Map config, where any faction character must rebirth. Skip if set to null.
        /// </summary>
        public RebirthConfiguration RebirthMap { get; set; }

        /// <summary>
        /// Map config, where light character must rebirth.
        /// </summary>
        public RebirthConfiguration LightRebirthMap { get; set; }

        /// <summary>
        /// Map config, where light character must rebirth.
        /// </summary>
        public RebirthConfiguration DarkRebirthMap { get; set; }
    }
}
