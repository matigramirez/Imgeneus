namespace Imgeneus.World.Game.Guild
{
    public enum GuildHouseBuyReason : byte
    {
        /// <summary>
        /// Guild house was purchased.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Guild has already bought guild house.
        /// </summary>
        AlreadyBought = 1,

        /// <summary>
        /// Not enough gold.
        /// </summary>
        NoGold = 2,

        /// <summary>
        /// Only guild master (rank 1) can buy guild house, all others are not authorized.
        /// </summary>
        NotAuthorized = 3,

        /// <summary>
        /// Guilds with rank >= 30 can have guild house.
        /// </summary>
        LowRank = 4
    }
}
