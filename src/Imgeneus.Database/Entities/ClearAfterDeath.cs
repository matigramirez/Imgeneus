namespace Imgeneus.Database.Entities
{
    public enum ClearAfterDeath : byte
    {
        /// <summary>
        /// Buff is cleared after death.
        /// </summary>
        Clear,

        /// <summary>
        /// Items such as "EXP stone", "Endurance" etc.
        /// </summary>
        KeepInHours,

        /// <summary>
        /// Items such as "Health Remedy".
        /// </summary>
        KeepInMins
    }
}
