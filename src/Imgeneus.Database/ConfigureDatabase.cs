using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imgeneus.Database
{
    public static class ConfigureDatabase
    {
        private const string MySqlConnectionString = "server=127.0.0.1;userid=root;pwd=your_password;port=3306;database=shaiya;sslmode=none;";

        public static DbContextOptionsBuilder ConfigureCorrectDatabase(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(MySqlConnectionString);
            return optionsBuilder;
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
