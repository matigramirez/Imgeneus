using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Imgeneus.World.Tests
{
    public abstract class BaseTest
    {
        public BaseTest()
        {
            var services = DependencyContainer.Instance.GetServiceCollection();
            services.AddDbContext<DatabaseContext>(o => o.UseInMemoryDatabase(databaseName: "testData"))
                    .AddTransient<IDatabase, DatabaseContext>();
            DependencyContainer.Instance.BuildServiceProvider();
        }
    }
}
