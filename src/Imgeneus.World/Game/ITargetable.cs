namespace Imgeneus.World.Game
{

    /// <summary>
    /// Special interface, that all targetable objects must implement.
    /// Targetable objects like: players, mobs, npc etc.
    /// </summary>
    public interface ITargetable
    {
        /// <summary>
        /// Unique id inside of a game world.
        /// </summary>
        public uint GlobalId { get; }

        /// <summary>
        /// Current health.
        /// </summary>
        public int CurrentHP { get; }
    }
}
