using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Imgeneus.Database
{
    public static class ConfigureDatabase
    {
        public static DbContextOptionsBuilder ConfigureCorrectDatabase(this DbContextOptionsBuilder optionsBuilder, DatabaseConfiguration configuration)
        {
            optionsBuilder.UseMySql(configuration.ToString(), new MySqlServerVersion(new Version(8, 0, 22)));
            return optionsBuilder;
        }

        public static IServiceCollection RegisterDatabaseServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddDbContext<DatabaseContext>(options =>
                {
                    var dbConfig = serviceCollection.BuildServiceProvider().GetService<IOptions<DatabaseConfiguration>>();
                    options.ConfigureCorrectDatabase(dbConfig.Value);
                }, ServiceLifetime.Transient)
                .AddTransient<IDatabase, DatabaseContext>();
        }
    }
}
