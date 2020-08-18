namespace Imgeneus.World.Game.PartyAndRaid
{
    public enum RaidDropType
    {
        /// <summary>
        /// Add drop goes 1 by 1 to every player.
        /// </summary>
        Group = 1,

        /// <summary>
        /// Drop is randomized.
        /// </summary>
        Random = 2,

        /// <summary>
        /// Drop is to leader.
        /// </summary>
        Leader = 4
    }
}
