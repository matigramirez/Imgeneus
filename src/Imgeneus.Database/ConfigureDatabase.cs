using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imgeneus.Database
{
    public static class ConfigureDatabase
    {
        private const string MsSqlConnectionString = "Data Source=127.0.0.1;Initial Catalog=PS_UserData;User ID=Shaiya;Password=Shaiya123";

        public static DbContextOptionsBuilder ConfigureCorrectDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(MsSqlConnectionString);
            return optionsBuilder;
        }

        public static IServiceCollection RegisterDatabaseFactory(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<DatabaseFactory>();
        }

        public static IServiceCollection RegisterDatabaseServices(this IServiceCollection serviceCollection,
            DatabaseConfiguration configuration)
        {
            return serviceCollection
                .AddSingleton(configuration)
                .AddDbContext<DatabaseContext>(options => options.ConfigureCorrectDatabase(), ServiceLifetime.Transient)
                .AddTransient<IDatabase, Database>();
        }
    }
}
