using System;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildRankingManager
    {
        /// <summary>
        /// Add points to some guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="points">points to add</param>
        public void AddPoints(int guildId, short points);

        /// <summary>
        /// Event, that is fired, when guild changes number of points
        /// </summary>
        public event Action<int, int> OnPointsChanged;

        /// <summary>
        /// Finds guild id with the max number of points.
        /// </summary>
        public int GetTopGuild();

        /// <summary>
        /// Finds guild points by guild id.
        /// </summary>
        public int GetGuildPoints(int guildId);
    }
}
