using System.Collections.Generic;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildHouseConfiguration
    {
        /// <summary>
        /// Amount of money needed for buying guild house.
        /// </summary>
        public int HouseBuyMoney { get; }

        /// <summary>
        /// Weekly fee, needed for a basic guild house.
        /// </summary>
        public int HouseKeepEtin { get; }

        /// <summary>
        /// Guild house npc infos.
        /// </summary>
        public IEnumerable<GuildHouseNpcInfo> NpcInfos { get; }

        /// <summary>
        /// Direct NPC ids. Same as in database.
        /// </summary>
        public IEnumerable<int> NpcIds { get; }
    }
}
