namespace Imgeneus.World.Game.Guild
{
    public enum GuildCreateFailedReason : byte
    {
        /// <summary>
        /// Default failed reason.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Guild was created.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Not enough gold.
        /// </summary>
        NotEnoughGold = 2,

        /// <summary>
        /// Not enough members in party.
        /// </summary>
        NotEnoughMembers = 3,

        /// <summary>
        /// Members do not have the right level.
        /// </summary>
        LevelLimit = 4,

        /// <summary>
        /// Members do not have the right mode.
        /// </summary>
        GrowingMode = 5,

        /// <summary>
        /// Guild name contains banned words.
        /// </summary>
        WrongName = 6,

        /// <summary>
        /// Some party member has rejected.
        /// </summary>
        PartyMemberRejected = 7,

        /// <summary>
        /// Party member is assigned to another guild.
        /// </summary>
        PartyMemberInAnotherGuild = 8,

        /// <summary>
        /// Party member left another guild not so long ago.
        /// </summary>
        PartyMemberGuildPenalty = 9,
    }
}
