using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.Logs;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Dyeing;
using Imgeneus.World.Game.Linking;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Notice;
using Imgeneus.World.Game.NPCs;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Imgeneus.World.Game.Zone.MapConfig;
using Imgeneus.World.Game.Zone.Obelisks;
using Imgeneus.World.SelectionScreen;
using InterServer.Client;
using InterServer.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Imgeneus.World
{
    public sealed class WorldServerStartup
    {
        /// <inheritdoc />
        public void ConfigureServices(IServiceCollection services)
        {
            // Add options.
            services.AddOptions<WorldConfiguration>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("WorldServer").Bind(settings));
            services.AddOptions<DatabaseConfiguration>()
               .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("Database").Bind(settings));
            services.AddOptions<InterServerConfig>()
               .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("InterServer").Bind(settings));

            services.RegisterDatabaseServices();

            services.AddSingleton<IInterServerClient, ISClient>();
            services.AddSingleton<IWorldServer, WorldServer>();
            services.AddSingleton<IGameWorld, GameWorld>();
            services.AddSingleton<ISelectionScreenFactory, SelectionScreenFactory>();
            services.AddSingleton<IMapsLoader, MapsLoader>();
            services.AddSingleton<IMapFactory, MapFactory>();
            services.AddSingleton<IMobFactory, MobFactory>();
            services.AddSingleton<INpcFactory, NpcFactory>();
            services.AddSingleton<IObeliskFactory, ObeliskFactory>();
            services.AddSingleton<ICharacterFactory, CharacterFactory>();
            services.AddSingleton<ICharacterConfiguration, CharacterConfiguration>((x) => CharacterConfiguration.LoadFromConfigFile());
            services.AddSingleton<IDatabasePreloader, DatabasePreloader>((x) =>
            {
                using (var scope = x.CreateScope())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabasePreloader>>();
                    var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
                    return new DatabasePreloader(logger, db);
                }
            });
            services.AddSingleton<IChatManager, ChatManager>();
            services.AddSingleton<INoticeManager, NoticeManager>();

            services.AddTransient<ILogsDatabase, LogsDbContext>();
            services.AddTransient<ILinkingManager, LinkingManager>();
            services.AddTransient<IDyeingManager, DyeingManager>();

            services.AddLogging(builder =>
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
            });

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<DatabaseWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IWorldServer worldServer, ILogsDatabase logsDb, IDatabase mainDb)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            mainDb.Migrate();
            logsDb.Migrate();
            worldServer.Start();
        }
    }
}
