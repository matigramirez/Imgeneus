namespace Imgeneus.World.Game.Duel
{
    public enum DuelResponse : byte
    {
        /// <summary>
        /// Opponent rejected duel.
        /// </summary>
        Rejected = 0,

        /// <summary>
        /// Opponent approved duel.
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Duel can not be sent to this character.
        /// </summary>
        NotAllowed = 2,

        /// <summary>
        /// Opponent didn't respond.
        /// </summary>
        NoResponse = 3
    }
}
