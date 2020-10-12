using Imgeneus.Core;
using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Helpers;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.Logs;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Dyeing;
using Imgeneus.World.Game.Linking;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone.MapConfig;
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
        private const string CharacterConfigFile = "config/character.json";

        /// <inheritdoc />
        public void Configure()
        {
            DependencyContainer.Instance
                .GetServiceCollection()
                .RegisterDatabaseServices();

            DependencyContainer.Instance.Register<ILogsDatabase, LogsDbContext>(ServiceLifetime.Transient);

            DependencyContainer.Instance.Register<IWorldServer, WorldServer>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register<IGameWorld, GameWorld>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register<IMapsLoader, MapsLoader>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register<IDatabasePreloader, DatabasePreloader>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register<IChatManager, ChatManager>(ServiceLifetime.Singleton);
            DependencyContainer.Instance.Register<ILinkingManager, LinkingManager>(ServiceLifetime.Transient);
            DependencyContainer.Instance.Register<IDyeingManager, DyeingManager>(ServiceLifetime.Transient);
            DependencyContainer.Instance.Configure(services => services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning);
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Trace);
#else
                builder.SetMinimumLevel(LogLevel.Information);
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
                var characterConfiguration = ConfigurationHelper.Load<CharacterConfiguration>(CharacterConfigFile);
                services.AddSingleton(worldConfiguration);
                services.AddSingleton(characterConfiguration);
            });
            DependencyContainer.Instance.Configure(services =>
            {
                services.AddSingleton<DatabaseWorker>();
                services.AddHostedService(provider => provider.GetService<DatabaseWorker>());
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            });
            DependencyContainer.Instance.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void Run()
        {
            var logger = DependencyContainer.Instance.Resolve<ILogger<WorldServerStartup>>();
            var server = DependencyContainer.Instance.Resolve<IWorldServer>();
            var logsDb = DependencyContainer.Instance.Resolve<ILogsDatabase>();
            logsDb.Migrate();
            var dbWorker = DependencyContainer.Instance.Resolve<DatabaseWorker>();
            dbWorker.StartAsync(new System.Threading.CancellationToken());

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
