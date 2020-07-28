using Imgeneus.Logs.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imgeneus.Logs
{
    public class LogsDbContext : DbContext, ILogsDatabase
    {
        /// <summary>
        /// Logs of chats.
        /// </summary>
        public DbSet<ChatLog> ChatLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=imgeneus.logs.db");
        }

        public void Migrate()
        {
            Database.Migrate();
        }
    }
}
