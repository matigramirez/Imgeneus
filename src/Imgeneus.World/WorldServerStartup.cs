using Imgeneus.Core;
using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Helpers;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database;
using Imgeneus.Network;
using Imgeneus.World.InternalServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Imgeneus.World
{
    public sealed class WorldServerStartup : IProgramStartup
    {
        private const string WorldConfigFile = "config/world.json";

        /// <inheritdoc />
        public void Configure()
        {
            PacketHandler<ISClient>.Initialize();
            PacketHandler<WorldClient>.Initialize();

            DependencyContainer.Instance
                .GetServiceCollection()
                .RegisterDatabaseServices();

            DependencyContainer.Instance.Register<IWorldServer, WorldServer>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Configure(services => services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning);
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Trace);
#else
                builder.SetMinimumLevel(LogLevel.Warning);
#endif
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            }));
            DependencyContainer.Instance.Configure(services =>
            {
                var worldConfiguration = ConfigurationHelper.Load<WorldConfiguration>(WorldConfigFile);
                services.AddSingleton(worldConfiguration);
            });
            DependencyContainer.Instance.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void Run()
        {
            var logger = DependencyContainer.Instance.Resolve<ILogger<WorldServerStartup>>();
            var server = DependencyContainer.Instance.Resolve<IWorldServer>();

            try
            {
                logger.LogInformation("Starting WorldServer...");
                server.Start();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"An unexpected error occured in WorldServer.");
#if DEBUG
                Console.ReadLine();
#endif
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DependencyContainer.Instance.Dispose();
            LogManager.Shutdown();
        }
    }
}
