using Imgeneus.World.Game.Player;
using System.Collections.Concurrent;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public interface IGameWorld
    {
        /// <summary>
        /// Connected players.
        /// </summary>
        BlockingCollection<Character> Players { get; }

        /// <summary>
        /// Loads player into game world.
        /// </summary>
        Character LoadPlayer(int characterId);
    }
}
