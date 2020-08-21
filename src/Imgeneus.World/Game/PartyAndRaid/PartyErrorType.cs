namespace Imgeneus.World.Game.PartyAndRaid
{
    public enum PartyErrorType
    {
        /// <summary>
        /// Raid does not exist.
        /// </summary>
        RaidNotFound = 12,

        /// <summary>
        /// Auto join is turned off.
        /// </summary>
        RaidNoAutoJoin = 13,

        /// <summary>
        /// Raid is full, no free space.
        /// </summary>
        RaidNoFreePlace = 14,
    }
}
