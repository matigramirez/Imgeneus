using Imgeneus.Core.Helpers;
using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imgeneus.Database
{
    public static class ConfigureDatabase
    {
        private const string DatabaseConfigFile = "config/database.json";

        public static DbContextOptionsBuilder ConfigureCorrectDatabase(this DbContextOptionsBuilder optionsBuilder, DatabaseConfiguration configuration)
        {
            optionsBuilder.UseMySql(configuration.ToString());
            return optionsBuilder;
        }

        public static IServiceCollection RegisterDatabaseServices(this IServiceCollection serviceCollection)
        {
            var dbConfig = ConfigurationHelper.Load<DatabaseConfiguration>(DatabaseConfigFile);
            return serviceCollection
                .AddSingleton(dbConfig)
                .AddDbContext<DatabaseContext>(options => options.ConfigureCorrectDatabase(dbConfig), ServiceLifetime.Transient)
                .AddTransient<IDatabase, DatabaseContext>();
        }
    }
}
