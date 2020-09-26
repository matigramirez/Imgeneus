using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.Portals
{
    public class PortalConfiguration
    {
        [JsonPropertyName("faction")]
        public byte Faction { get; set; }

        [JsonPropertyName("minLvl")]
        public ushort MinLvl { get; set; }

        [JsonPropertyName("maxLvl")]
        public ushort MaxLvl { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("z")]
        public float Z { get; set; }

        [JsonPropertyName("destination")]
        public DestinationConfiguration Destination { get; set; }
    }

    public class DestinationConfiguration
    {
        [JsonPropertyName("mapId")]
        public ushort MapId { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("z")]
        public float Z { get; set; }
    }
}