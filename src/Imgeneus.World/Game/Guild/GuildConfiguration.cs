using Imgeneus.Core.Helpers;

namespace Imgeneus.World.Game.Guild
{
    public class GuildConfiguration : IGuildConfiguration
    {
        private const string ConfigFile = "config/Guild.json";

        public static GuildConfiguration LoadFromConfigFile()
        {
            return ConfigurationHelper.Load<GuildConfiguration>(ConfigFile);
        }

        /// <inheritdoc/>
        public byte MinMembers { get; set; }

        /// <inheritdoc/>
        public ushort MinLevel { get; set; }

        /// <inheritdoc/>
        public uint MinGold { get; set; }

        /// <inheritdoc/>
        public ushort MinPenalty { get; set; }
    }
}
