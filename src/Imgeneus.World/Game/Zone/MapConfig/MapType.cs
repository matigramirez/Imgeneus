using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum MapType
    {
        Field,
        Dungeon
    }
}