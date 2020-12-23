using Imgeneus.Core.Structures.Configuration;
using Imgeneus.Database;
using InterServer.Server;
using InterServer.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Imgeneus.Login
{
    public class LoginServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add options.
            services.AddOptions<LoginConfiguration>()
                .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("LoginServer").Bind(settings));
            services.AddOptions<DatabaseConfiguration>()
               .Configure<IConfiguration>((settings, configuration) => configuration.GetSection("Database").Bind(settings));

            services.RegisterDatabaseServices();

            services.AddSignalR();

            services.AddSingleton<IInterServer, ISServer>();
            services.AddSingleton<ILoginServer, LoginServer>();
            services.AddSingleton<ILoginManagerFactory, LoginManagerFactory>();
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoginServer loginServer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ISHub>("/inter_server");
            });

            loginServer.Start();
        }
    }
}
