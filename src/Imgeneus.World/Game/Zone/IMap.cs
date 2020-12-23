using Imgeneus.World.Game.Player;

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
    }
}
