using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.World.Game.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imgeneus.World.Game
{
    /// <summary>
    /// The virtual representation of game world.
    /// </summary>
    public class GameWorld : IGameWorld
    {
        private readonly ILogger<GameWorld> _logger;

        public GameWorld()
        {
            _logger = DependencyContainer.Instance.Resolve<ILogger<GameWorld>>();
        }

        #region Players

        /// <inheritdoc />
        public BlockingCollection<Character> Players { get; private set; } = new BlockingCollection<Character>();

        /// <inheritdoc />
        public Character LoadPlayer(int characterId)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbCharacter = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                               .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                               .FirstOrDefault(c => c.Id == characterId);
            var newPlayer = Character.FromDbCharacter(dbCharacter);

            Players.Add(newPlayer);
            _logger.LogDebug($"Player {newPlayer.Id} connected to game world");
            return newPlayer;
        }

        #endregion
    }
}
