using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.Portals;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public interface IGameWorld
    {
        /// <summary>
        /// Connected players. Key is character id, value is character.
        /// </summary>
        ConcurrentDictionary<int, Character> Players { get; }

        /// <summary>
        /// Loaded maps. Key is map id, value is map.
        /// </summary>
        ConcurrentDictionary<ushort, IMap> Maps { get; }

        /// <summary>
        /// Collection of map ids, that are available for GM teleport.
        /// </summary>
        IList<ushort> AvailableMapIds { get; }

        /// <summary>
        /// Ensures, that character can be loaded to map, that we got from db.
        /// NB! Mutates dbCharacter, if he can not be loaded to map for some reason!
        /// Reason can be next: map was deleted from the server, map was instance map, something went wrong and we saved wrong map id in database.
        /// </summary>
        /// <param name="dbCharacter"></param>
        void EnsureMap(DbCharacter dbCharacter);

        /// <summary>
        /// Loads player into game world.
        /// </summary>
        /// <param name="characterId">id of character in database</param>
        /// <param name="client">TCP connection with client</param>
        /// <returns>character, that is loaded into game world</returns>
        Task<Character> LoadPlayer(int characterId, WorldClient client);

        /// <summary>
        /// Loads player into map and send notification other players.
        /// </summary>
        void LoadPlayerInMap(int characterId);

        /// <summary>
        /// Removes player from game world.
        /// </summary>
        void RemovePlayer(int characterId);

        /// <summary>
        /// Checks, if player can be teleported with this portal. I.e. checks level, money, if map is open etc.
        /// </summary>
        /// <param name="player">player to teleport</param>
        /// <param name="portalIndex">portal index</param>
        /// <param name="reason">optional out param, that indicates the reason why teleport is not allowed for this character</param>
        /// <returns>true, if it can teleport</returns>
        bool CanTeleport(Character player, byte portalIndex, out PortalTeleportNotAllowedReason reason);
    }
}
