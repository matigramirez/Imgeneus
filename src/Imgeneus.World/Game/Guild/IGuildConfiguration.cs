namespace Imgeneus.World.Game.Guild
{
    public interface IGuildConfiguration
    {
        /// <summary>
        /// Min number of players needed for guild creation.
        /// </summary>
        public byte MinMembers { get; }

        /// <summary>
        /// Min level of players needed for guild creation.
        /// </summary>
        public ushort MinLevel { get; }

        /// <summary>
        /// Min gold needed for guild creation.
        /// </summary>
        public uint MinGold { get; }

        /// <summary>
        /// Min number of hours since player since player created guild.
        /// </summary>
        public ushort MinPenalty { get; }
    }
}
