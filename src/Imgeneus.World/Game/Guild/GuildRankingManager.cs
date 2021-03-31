using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public class GuildRankingManager : IGuildRankingManager
    {
        private readonly IDatabase _database;

        private SemaphoreSlim _sync = new SemaphoreSlim(1);

        public GuildRankingManager(IDatabase database)
        {
            _database = database;
        }

        /// <inheritdoc/>
        public event Action<int, int> OnPointsChanged;

        /// <inheritdoc/>
        public async Task AddPoints(int guildId, short points)
        {
            await _sync.WaitAsync();

            var guild = await _database.Guilds.FindAsync(guildId);
            if (guild is null)
                return;

            guild.Points += points;

            OnPointsChanged?.Invoke(guild.Id, guild.Points);

            await _database.SaveChangesAsync();

            _sync.Release();
        }

        /// <inheritdoc/>
        public IEnumerable<DbGuild> GetTopGuilds(int count)
        {
            return _database.Guilds.OrderByDescending(x => x.Points).Take(count);
        }

        /// <inheritdoc/>
        public DbGuild GetGuild(int guildId)
        {
            return _database.Guilds.Find(guildId);
        }
    }
}
