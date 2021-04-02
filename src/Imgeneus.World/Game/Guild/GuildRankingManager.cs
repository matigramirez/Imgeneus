using Imgeneus.Database;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game.Guild
{
    public class GuildRankingManager : IGuildRankingManager
    {
        private readonly IDatabase _database;

        public GuildRankingManager(IDatabase database)
        {
            _database = database;
        }

        /// <inheritdoc/>
        public event Action<int, int> OnPointsChanged;

        /// <summary>
        /// During GRB all guild points saved here.
        /// Key is guild id. Value is points.
        /// </summary>
        private readonly ConcurrentDictionary<int, int> GuildPoints = new ConcurrentDictionary<int, int>();

        /// <inheritdoc/>
        public void AddPoints(int guildId, short points)
        {
            if (!GuildPoints.ContainsKey(guildId))
                GuildPoints[guildId] = points;
            else
                GuildPoints[guildId] += points;

            OnPointsChanged?.Invoke(guildId, GuildPoints[guildId]);
        }

        /// <inheritdoc/>
        public int GetTopGuild()
        {
            var key = GuildPoints.OrderByDescending(x => x.Value).Select(x => x.Key).FirstOrDefault();
            return key;
        }

        /// <inheritdoc/>
        public int GetGuildPoints(int guildId)
        {
            return GuildPoints[guildId];
        }
    }
}
