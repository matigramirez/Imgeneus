using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Preload;
using Imgeneus.DatabaseBackgroundService;
using Imgeneus.World.Game.Chat;
using Imgeneus.World.Game.Dyeing;
using Imgeneus.World.Game.Linking;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.NPCs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Imgeneus.World.Game.Notice;

namespace Imgeneus.World.Game.Player
{
    public class CharacterFactory : ICharacterFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<ICharacterFactory> _logger;

        public CharacterFactory(IServiceProvider provider, ILogger<ICharacterFactory> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task<Character> CreateCharacter(int characterId, WorldClient client)
        {
            using var scope = _provider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            using var database = scopedProvider.GetService<IDatabase>();

            var dbCharacter = await database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                                        .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                                        .Include(c => c.ActiveBuffs).ThenInclude(cb => cb.Skill)
                                                        .Include(c => c.Friends).ThenInclude(cf => cf.Friend)
                                                        .Include(c => c.Quests)
                                                        .Include(c => c.QuickItems)
                                                        .Include(c => c.User)
                                                        .FirstOrDefaultAsync(c => c.Id == characterId);

            if (dbCharacter is null)
            {
                _logger.LogWarning($"Character with id {characterId} is not found.");
                return null;
            }

            Character.ClearOutdatedValues(database, dbCharacter);
            scopedProvider.GetService<IGameWorld>().EnsureMap(dbCharacter);

            var player = Character.FromDbCharacter(dbCharacter,
                                        scopedProvider.GetService<ILogger<Character>>(),
                                        scopedProvider.GetService<IGameWorld>(),
                                        scopedProvider.GetService<ICharacterConfiguration>(),
                                        scopedProvider.GetService<IBackgroundTaskQueue>(),
                                        scopedProvider.GetService<IDatabasePreloader>(),
                                        scopedProvider.GetService<IChatManager>(),
                                        scopedProvider.GetService<ILinkingManager>(),
                                        scopedProvider.GetService<IDyeingManager>(),
                                        scopedProvider.GetService<IMobFactory>(),
                                        scopedProvider.GetService<INpcFactory>(),
                                        scopedProvider.GetService<INoticeManager>());
            player.Client = client;
            return player;
        }
    }
}
