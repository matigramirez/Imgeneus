using Imgeneus.Logs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.Logs
{
    public interface ILogsDatabase : IDisposable
    {
        /// <summary>
        /// Logs of chats.
        /// </summary>
        DbSet<ChatLog> ChatLogs { get; set; }

        /// <summary>
        /// Database migration.
        /// </summary>
        void Migrate();

        /// <summary>
        /// Saves changes to database.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
