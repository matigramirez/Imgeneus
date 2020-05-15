namespace Imgeneus.World.Game
{
    /// <summary>
    /// Special interface, that all killers must implement.
    /// Killer can be another player, npc or mob.
    /// </summary>
    public interface IKiller
    {
        /// <summary>
        /// Unique id inside of a game world.
        /// </summary>
        public int Id { get; }
    }
}
