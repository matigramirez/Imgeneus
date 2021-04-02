namespace Imgeneus.World.Game.Guild
{
    public enum GRBNotice : byte
    {
        /// <summary>
        /// GRB will start soon.
        /// </summary>
        StartsSoon = 0,

        /// <summary>
        /// GRB just started.
        /// </summary>
        Started = 100,

        /// <summary>
        /// 10 mins left.
        /// </summary>
        Min10 = 101,

        /// <summary>
        /// 1 min left
        /// </summary>
        Min1 = 102
    }
}
