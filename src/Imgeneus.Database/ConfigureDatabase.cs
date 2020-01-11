using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imgeneus.Database
{
    public static class ConfigureDatabase
    {
        public static DbContextOptionsBuilder ConfigureCorrectDatabase(this DbContextOptionsBuilder optionsBuilder, DatabaseConfiguration configuration)
        {
            optionsBuilder.UseMySql(configuration.ToString());
            return optionsBuilder;
        }

        public static IServiceCollection RegisterDatabaseServices(this IServiceCollection serviceCollection,
            DatabaseConfiguration configuration)
        {
            return serviceCollection
                .AddSingleton(configuration)
                .AddDbContext<DatabaseContext>(options => options.ConfigureCorrectDatabase(configuration), ServiceLifetime.Transient)
                .AddTransient<IDatabase, Database>();
        }
    }
}
