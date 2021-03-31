using Imgeneus.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildRankingManager
    {
        /// <summary>
        /// Add points to some guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="points">points to add</param>
        public Task AddPoints(int guildId, short points);

        /// <summary>
        /// Event, that is fired, when guild changes number of points
        /// </summary>
        public event Action<int, int> OnPointsChanged;

        /// <summary>
        /// Selects top X guilds ordered bypoints.
        /// </summary>
        /// <param name="count">top X</param>
        /// <returns>collection of guilds</returns>
        public IEnumerable<DbGuild> GetTopGuilds(int count);

        /// <summary>
        /// Finds guild by id.
        /// </summary>
        public DbGuild GetGuild(int guildId);
    }
}
