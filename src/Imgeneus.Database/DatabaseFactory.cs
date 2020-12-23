using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Imgeneus.Database
{
    /// <summary>
    /// Factory used for db migrations.
    /// </summary>
    public class DatabaseFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", optional: true)
#else
                .AddJsonFile($"appsettings.Release.json", optional: true)
#endif
                .Build();

            var dbConfig = new DatabaseConfiguration();
            configuration.Bind("Database", dbConfig);

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.ConfigureCorrectDatabase(dbConfig);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
