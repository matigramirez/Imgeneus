using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class NamedAreaConfiguration
    {
        [JsonPropertyName("x1")]
        public float X1 { get; set; }

        [JsonPropertyName("y1")]
        public float Y1 { get; set; }

        [JsonPropertyName("z1")]
        public float Z1 { get; set; }

        [JsonPropertyName("x2")]
        public float X2 { get; set; }

        [JsonPropertyName("y2")]
        public float Y2 { get; set; }

        [JsonPropertyName("z2")]
        public float Z2 { get; set; }

        [JsonPropertyName("param1")]
        public int Param1 { get; set; }

        [JsonPropertyName("param2")]
        public int Param2 { get; set; }
    }
}