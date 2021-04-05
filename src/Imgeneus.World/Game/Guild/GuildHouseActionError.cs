namespace Imgeneus.World.Game.Guild
{
    public enum GuildHouseActionError : byte
    {
        /// <summary>
        /// No error.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Guild has too low rank in order to use NPC. Guild needs to participate in GRB.
        /// </summary>
        LowRank = 1,

        /// <summary>
        /// NPC level is not high enough. Guild need to buy this NPC level first.
        /// </summary>
        LowLevel = 2
    }
}
