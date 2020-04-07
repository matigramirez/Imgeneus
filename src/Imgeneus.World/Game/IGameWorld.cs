using Imgeneus.World.Game.Player;
using System;
using System.Collections.Concurrent;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public interface IGameWorld
    {
        /// <summary>
        /// Event, that is fired, when new player enters the map.
        /// </summary>
        public event Action<Character> OnPlayerEnteredMap;

        /// <summary>
        /// Connected players.
        /// </summary>
        BlockingCollection<Character> Players { get; }

        /// <summary>
        /// Loads player into game world.
        /// </summary>
        Character LoadPlayer(int characterId);

        /// <summary>
        /// Loads player into map and send notification other players.
        /// </summary>
        Character LoadPlayerInMap(int characterId);
    }
}
