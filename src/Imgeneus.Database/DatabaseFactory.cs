using Imgeneus.Core.Helpers;
using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Imgeneus.Database
{
    /// <summary>
    /// Factory used for db migrations.
    /// </summary>
    public class DatabaseFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            var dbConfig = ConfigurationHelper.Load<DatabaseConfiguration>("config/database.json");
            optionsBuilder.ConfigureCorrectDatabase(dbConfig);

            return new DatabaseContext(optionsBuilder.Options);
        }

        public static IDatabase GetDatabase()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
            var dbConfig = ConfigurationHelper.Load<DatabaseConfiguration>("config/database.json");
            optionsBuilder.ConfigureCorrectDatabase(dbConfig);
            DatabaseContext databaseContext = new DatabaseContext(optionsBuilder.Options);
            return new Database(databaseContext);
        }
    }
}
