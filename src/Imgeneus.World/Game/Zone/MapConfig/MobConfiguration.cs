using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class MobConfiguration
    {
        [JsonPropertyName("mobId")]
        public ushort MobId { get; set; }

        [JsonPropertyName("mobCount")]
        public int MobCount { get; set; }
    }
}