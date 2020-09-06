using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.Obelisks
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum ObeliskCountry : byte
    {
        None,
        Light,
        Dark
    }
}