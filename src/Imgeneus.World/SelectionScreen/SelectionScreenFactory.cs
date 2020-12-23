using Imgeneus.Database;
using Imgeneus.World.Game;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Imgeneus.World.SelectionScreen
{
    public class SelectionScreenFactory : ISelectionScreenFactory
    {
        private readonly IServiceProvider _provider;

        public SelectionScreenFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ISelectionScreenManager CreateSelectionManager(WorldClient client)
        {
            var scope = _provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            return new SelectionScreenManager(client, scopedProvider.GetService<IGameWorld>(), scopedProvider.GetService<ICharacterConfiguration>(), scopedProvider.GetService<IDatabase>());
        }
    }
}
