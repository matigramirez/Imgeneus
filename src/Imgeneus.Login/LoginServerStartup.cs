using Imgeneus.Core;
using Imgeneus.Core.DependencyInjection;
using Imgeneus.Core.Helpers;
using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database;
using Imgeneus.Login.InternalServer;
using Imgeneus.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Imgeneus.Login
{
    public sealed class LoginServerStartup : IProgramStartup
    {
        private const string LoginConfigFile = "config/login.json";

        /// <inheritdoc />
        public void Configure()
        {
            DependencyContainer.Instance
                .GetServiceCollection()
                .RegisterDatabaseServices();

            DependencyContainer.Instance.Register<ILoginServer, LoginServer>(ServiceLifetime.Singleton);
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
                var loginConfiguration = ConfigurationHelper.Load<LoginConfiguration>(LoginConfigFile);
                services.AddSingleton(loginConfiguration);
            });
            DependencyContainer.Instance.BuildServiceProvider();
        }

        /// <inheritdoc />
        public void Run()
        {
            var logger = DependencyContainer.Instance.Resolve<ILogger<LoginServerStartup>>();
            var server = DependencyContainer.Instance.Resolve<ILoginServer>();
            try
            {
                logger.LogInformation("Starting LoginServer...");
                server.Start();

                Console.ReadLine();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"An unexpected error occured in LoginServer.");
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
