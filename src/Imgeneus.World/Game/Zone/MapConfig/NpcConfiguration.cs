using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    public class NpcConfiguration
    {
        [JsonPropertyName("type")]
        public byte Type { get; set; }

        [JsonPropertyName("typeId")]
        public ushort TypeId { get; set; }

        [JsonPropertyName("coordinates")]
        public List<NpcCoordinate> Coordinates { get; set; }
    }
}