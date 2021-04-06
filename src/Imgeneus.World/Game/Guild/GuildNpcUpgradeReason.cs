namespace Imgeneus.World.Game.Guild
{
    public enum GuildNpcUpgradeReason : byte
    {
        /// <summary>
        /// No issues.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Not enough etin.
        /// </summary>
        NoEtin = 1,

        /// <summary>
        /// Upgrade can be only 1 by 1 level.
        /// </summary>
        OneByOneLvl = 2,

        /// <summary>
        /// Unknown reason.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Failed due to guild low rank in GRB.
        /// </summary>
        LowRank = 4
    }
}
