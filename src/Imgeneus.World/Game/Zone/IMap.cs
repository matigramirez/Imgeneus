using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using Imgeneus.World.Game.Zone.Portals;

namespace Imgeneus.World.Game.Zone
{
    public interface IMap
    {
        /// <summary>
        /// Map must have unique id.
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        /// Loads player into the map.
        /// </summary>
        public bool LoadPlayer(Character player);

        /// <summary>
        /// Removes player from the map.
        /// </summary>
        public bool UnloadPlayer(Character player);

        /// <summary>
        /// Map portals.
        /// </summary>
        public IList<Portal> Portals { get; }

        /// <summary>
        /// Finds the nearest spawn for the player.
        /// </summary>
        /// <param name="currentX">current player x coordinate</param>
        /// <param name="currentY">current player y coordinate</param>
        /// <param name="currentZ">current player z coordinate</param>
        /// <param name="fraction">player's faction</param>
        /// <returns>coordinate, where player shoud spawn</returns>
        public (float X, float Y, float Z) GetNearestSpawn(float currentX, float currentY, float currentZ, Fraction fraction);

        /// <summary>
        /// Is this map created for party, guild etc. ?
        /// </summary>
        public bool IsInstance { get; }
    }
}
