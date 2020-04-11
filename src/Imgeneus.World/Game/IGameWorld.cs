using Imgeneus.Database.Constants;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
        /// Event, that is fired, when player leaves the map.
        /// </summary>
        public event Action<Character> OnPlayerLeftMap;

        /// <summary>
        /// Event, that is fired, when player sends motion.
        /// </summary>
        public event Action<int, Motion> OnPlayerMotion;

        /// <summary>
        /// Event, that is fired, when player gets buff.
        /// </summary>
        public event Action<Character, ActiveBuff> OnPlayerGotBuff;

        /// <summary>
        /// Event, that is fired, when player uses skill.
        /// </summary>
        public event Action<Character, Skill> OnPlayerUsedSkill;

        /// <summary>
        /// Connected players. Key is character id, value is character.
        /// </summary>
        ConcurrentDictionary<int, Character> Players { get; }

        /// <summary>
        /// Loads player into game world.
        /// </summary>
        Character LoadPlayer(int characterId);

        /// <summary>
        /// Loads player into map and send notification other players.
        /// </summary>
        Character LoadPlayerInMap(int characterId);

        /// <summary>
        /// Removes player from game world.
        /// </summary>
        void RemovePlayer(int characterId);

        /// <summary>
        /// Event, that is fired, when the player moves.
        /// </summary>
        public event Action<Character> OnPlayerMove;

        /// <summary>
        /// Updates player's position and notifies aboyt this update.
        /// </summary>
        /// <param name="characterId">id of character</param>
        /// <param name="movementType">running or stopped</param>
        /// <param name="X">x coordinate</param>
        /// <param name="Y">y coordinate</param>
        /// <param name="Z">z coordinate</param>
        /// <param name="angle">rotation angle</param>
        Task PlayerMoves(int characterId, MovementType movementType, float X, float Y, float Z, ushort angle);

        /// <summary>
        /// Character sends some motion.
        /// </summary>
        /// <param name="characterId">id of character</param>
        /// <param name="motion">motion type</param>
        void PlayerSendMotion(int characterId, Motion motion);

        /// <summary>
        /// Character used some skill.
        /// </summary>
        /// <param name="characterId">id of character</param>
        /// <param name="skillNumber">unique number of skill; unique is per character(maybe?)</param>
        Task PlayerUsedSkill(int characterId, byte skillNumber);

        /// <summary>
        /// GM command, that creates mob near the character.
        /// </summary>
        /// <param name="characterId">character id, near which mobs are going to be created</param>
        /// <param name="mobId">mob id</param>
        void GMCreateMob(int characterId, ushort mobId);

        /// <summary>
        /// TODO: move it to map.
        /// Event, that is fired, when mob enters the map.
        /// </summary>
        public event Action<Mob> OnMobEnter;

        /// <summary>
        /// TODO: move it to map.
        /// Event, that is fired, when mob moves to new position.
        /// </summary>
        public event Action<Mob> OnMobMove;

        /// <summary>
        /// TODO: move it to map.
        /// Event, that is fired, when mob attacks the player.
        /// </summary>
        public event Action<Mob, int> OnMobAttack;

        /// <summary>
        /// Gets mob by its' id.
        /// </summary>
        /// <param name="characterId">characted id is possibly needed to get map id</param>
        /// <param name="mobId">mob id</param>
        public Mob GetMob(int characterId, uint mobId);
    }
}
