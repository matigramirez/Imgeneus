using Imgeneus.Database;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Imgeneus.Login
{
    public class LoginManagerFactory : ILoginManagerFactory
    {
        private readonly IServiceProvider _provider;

        public LoginManagerFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public LoginManager CreateLoginManager(LoginClient client, ILoginServer server)
        {
            var scope = _provider.CreateScope();

            return new LoginManager(client, server, scope.ServiceProvider.GetRequiredService<IDatabase>());
        }
    }
}
