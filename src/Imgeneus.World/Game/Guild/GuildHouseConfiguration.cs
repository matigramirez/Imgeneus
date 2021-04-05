using Imgeneus.Core.Helpers;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Guild
{
    public class GuildHouseConfiguration : IGuildHouseConfiguration
    {
        private const string ConfigFile = "config/GuildHouse.json";

        public static GuildHouseConfiguration LoadFromConfigFile()
        {
            return ConfigurationHelper.Load<GuildHouseConfiguration>(ConfigFile);
        }

        /// <inheritdoc/>
        public int HouseBuyMoney { get; set; }

        /// <inheritdoc/>
        public int HouseKeepEtin { get; set; }

        /// <inheritdoc/>
        public IEnumerable<GuildHouseNpcInfo> NpcInfos { get; set; }
    }
}
