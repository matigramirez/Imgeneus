namespace Imgeneus.World.Game
{
    /// <summary>
    /// Must-have stats.
    /// </summary>
    public interface IStatsHolder
    {
        /// <summary>
        /// Luck value, needed for critical damage calculation.
        /// </summary>
        public int TotalLuc { get; }

        /// <summary>
        /// Wis value, needed for damage calculation.
        /// </summary>
        public int TotalWis { get; }

        /// <summary>
        /// Dex value, needed for damage calculation.
        /// </summary>
        public int TotalDex { get; }

        /// <summary>
        /// Physical defense.
        /// </summary>
        public int Defense { get; }

        /// <summary>
        /// Magic resistance.
        /// </summary>
        public int Resistance { get; }

        /// <summary>
        /// Possibility to hit enemy.
        /// </summary>
        public double PhysicalHittingChance { get; }

        /// <summary>
        /// Possibility to escape hit.
        /// </summary>
        public double PhysicalEvasionChance { get; }

        /// <summary>
        /// Possibility to magic hit enemy.
        /// </summary>
        public double MagicHittingChance { get; }

        /// <summary>
        /// Possibility to escape magic hit.
        /// </summary>
        public double MagicEvasionChance { get; }

        /// <summary>
        /// Possibility to make critical hit.
        /// </summary>
        public double CriticalHittingChance { get; }
    }
}
