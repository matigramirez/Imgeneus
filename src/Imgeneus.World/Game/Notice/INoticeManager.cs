using Imgeneus.Database.Entities;

namespace Imgeneus.World.Game.Notice
{
    public interface INoticeManager
    {
        /// <summary>
        /// Sends a notice to all online players.
        /// Optionally, the notice can be repeated periodically specifying a time interval in seconds.
        /// </summary>
        /// <param name="message">Notice message</param>
        /// <param name="timeInterval">Time interval in seconds</param>
        public void SendWorldNotice(string message, short timeInterval = 0);

        /// <summary>
        /// Sends a notice to all online players that belong to a specified faction.
        /// Optionally, the notice can be repeated periodically specifying a time interval in seconds.
        /// </summary>
        /// <param name="message">Notice message</param>
        /// <param name="faction">Target faction</param>
        /// <param name="timeInterval">Time interval in seconds</param>
        public void SendFactionNotice(string message, Fraction faction, short timeInterval = 0);

        /// <summary>
        /// Sends a notice to all online players that are located in a specific map.
        /// Optionally, the notice can be repeated periodically specifying a time interval in seconds.
        /// </summary>
        /// <param name="message">Notice message</param>
        /// <param name="mapId">Target map</param>
        /// <param name="timeInterval">Time interval in seconds</param>
        public void SendMapNotice(string message, ushort mapId, short timeInterval = 0);

        /// <summary>
        /// Attempts to send a notice to a specific player.
        /// Optionally, the notice can be repeated periodically specifying a time interval in seconds.
        /// </summary>
        /// <param name="message">Notice message</param>
        /// <param name="targetPlayer">Target player's name</param>
        /// <param name="timeInterval">Time interval in seconds</param>
        /// <returns>Success status indicating whether the notice was sent or not. It will fail when the target player is offline.</returns>
        public bool TrySendPlayerNotice(string message, string targetPlayer, short timeInterval = 0);

        /// <summary>
        /// Sends a notice to all online admin players.
        /// </summary>
        /// <param name="message">Notice message</param>
        public void SendAdminNotice(string message);

        /// <summary>
        /// Sends a notice to all online players within the map cell that contains specific coordinates.
        /// </summary>
        /// <param name="message">Notice message</param>
        /// <param name="posX">X coordinate</param>
        /// <param name="posZ">Z coordinate</param>
        public void SendAreaNotice(string message, ushort posX, ushort posZ);
    }
}