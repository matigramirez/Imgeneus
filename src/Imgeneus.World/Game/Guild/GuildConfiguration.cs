namespace Imgeneus.World.Game.Guild
{
    public class GuildConfiguration
    {
        /// <summary>
        /// Min number of players needed for guild creation.
        /// </summary>
        public byte MinMembers { get; set; }

        /// <summary>
        /// Min level of players needed for guild creation.
        /// </summary>
        public ushort MinLevel { get; set; }

        /// <summary>
        /// Min gold needed for guild creation.
        /// </summary>
        public uint MinGold { get; set; }

        /// <summary>
        /// Min number of hours since player since player created guild.
        /// </summary>
        public ushort MinPenalty { get; set; }

        /// <summary>
        /// Amount of money needed for buying guild house.
        /// </summary>
        public int HouseBuyMoney { get; set; }

        /// <summary>
        /// Weekly fee, needed for a basic guild house.
        /// </summary>
        public int HouseKeepEtin { get; set; }
    }
}
