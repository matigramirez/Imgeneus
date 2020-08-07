namespace Imgeneus.World.Game.Duel
{
    public enum DuelCancelReason : byte
    {
        /// <summary>
        /// Player was disconnected.
        /// </summary>
        OpponentDisconnected = 0,

        /// <summary>
        /// Player is too far away from duel starting point.
        /// </summary>
        TooFarAway = 1,

        /// <summary>
        /// Player admits defeat.
        /// </summary>
        AdmitDefeat = 2,

        /// <summary>
        /// Duel was canceled, because of mob/another player(not opponent) attack.
        /// </summary>
        MobAttack = 3,

        /// <summary>
        /// When other, duel is canceled with no reason.
        /// </summary>
        Other = 4,

        /// <summary>
        /// Player lost this duel.
        /// </summary>
        Lose = 10,
    }
}
