using System.Text.Json.Serialization;

namespace Imgeneus.World.Game.Zone.MapConfig
{
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum CreateType : byte
    {
        /// <summary>
        /// Map is created 1 for all.
        /// </summary>
        Default,

        /// <summary>
        /// Map is created for party.
        /// </summary>
        Party,

        /// <summary>
        /// Map is created for guild.
        /// </summary>
        Guild,

        /// <summary>
        /// Map is created for Guilds Rank Battle (GRB).
        /// </summary>
        GRB,
    }
}