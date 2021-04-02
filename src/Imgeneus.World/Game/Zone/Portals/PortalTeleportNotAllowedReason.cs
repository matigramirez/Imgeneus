namespace Imgeneus.World.Game.Zone.Portals
{
    public enum PortalTeleportNotAllowedReason : byte
    {
        /// <summary>
        /// Only guild members can enter this map.
        /// </summary>
        OnlyForGuilds = 0,

        /// <summary>
        /// Guild battle is closed.
        /// </summary>
        NotTimeForRankingBattle = 1,

        /// <summary>
        /// Player has already participated in the battle.
        /// </summary>
        AlreadyParticipatedInBattle = 2,

        /// <summary>
        /// Only members of top 30 guilds are allowed to enter this map.
        /// </summary>
        OnlyTop30Guilds = 3,

        /// <summary>
        /// Only party members are allowed to enter this map.
        /// </summary>
        OnlyForParty = 4,

        /// <summary>
        /// Sends question if character wants to participate in guild battle.
        /// </summary>
        PaticipateInBattleQuestion = 5,

        /// <summary>
        /// Only for guilds, that paid their guild house.
        /// </summary>
        NoGuildHouse = 6,

        /// <summary>
        /// Can enter the guild house after 5 days of admission.
        /// </summary>
        GuildHouse5DaysAdmission = 7,

        /// <summary>
        /// The maintenance fee is not paid.
        /// </summary>
        FeeNotPaid = 8,

        /// <summary>
        /// Map is open only for party and in time frame. E.g. oblivion insula map.
        /// </summary>
        OnlyForPartyAndOnTime = 9,

        /// <summary>
        /// Can not enter the map, because of the lack of party members.
        /// </summary>
        NotEnoughPartyMembers = 10,

        /// <summary>
        /// Any other reason.
        /// </summary>
        Unknown = 11,

        /// <summary>
        /// Can not join goddess competition.
        /// </summary>
        NoGoddessCompetition = 12
    }
}
