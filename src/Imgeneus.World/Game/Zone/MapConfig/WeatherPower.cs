using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum WeatherPower : byte
    {
        Light = 1,
        Normal = 2,
        Hard = 3
    }
}