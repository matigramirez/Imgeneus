using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Zone;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

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

            InitMaps();
        }

        #region Maps 

        /// <summary>
        /// Thread-safe dictionary of maps. Where key is map id.
        /// </summary>
        public ConcurrentDictionary<ushort, Map> Maps { get; private set; } = new ConcurrentDictionary<ushort, Map>();

        /// <summary>
        /// Initializes maps with startup values like mobs, npc, areas, obelisks etc.
        /// </summary>
        private void InitMaps()
        {
            // TODO: init maps here. For now create 0-map(DWaterBorderland, Lvl 40-80)
            Maps.TryAdd(0, new Map(0, DependencyContainer.Instance.Resolve<ILogger<Map>>()));
        }

        #endregion

        #region Players

        /// <inheritdoc />
        public ConcurrentDictionary<int, Character> Players { get; private set; } = new ConcurrentDictionary<int, Character>();

        /// <inheritdoc />
        public Character LoadPlayer(int characterId, WorldClient client)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbCharacter = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                               .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                               .Include(c => c.ActiveBuffs).ThenInclude(cb => cb.Skill)
                                               .Include(c => c.User)
                                               .FirstOrDefault(c => c.Id == characterId);
            var newPlayer = Character.FromDbCharacter(dbCharacter, DependencyContainer.Instance.Resolve<ILogger<Character>>());
            newPlayer.Client = client;

            Players.TryAdd(newPlayer.Id, newPlayer);
            _logger.LogDebug($"Player {newPlayer.Id} connected to game world");

            return newPlayer;
        }

        /// <inheritdoc />
        public async Task PlayerMoves(int characterId, MovementType movementType, float X, float Y, float Z, ushort angle)
        {
            var player = Players[characterId];
            var map = Maps[player.Map];
            await map.PlayerMoves(characterId, movementType, X, Y, Z, angle);
        }

        /// <inheritdoc />
        public void LoadPlayerInMap(int characterId)
        {
            var player = Players[characterId];
            Maps[player.Map].LoadPlayer(player);
        }

        /// <inheritdoc />
        public void RemovePlayer(int characterId)
        {
            Character player;
            if (Players.TryRemove(characterId, out player))
            {
                _logger.LogDebug($"Player {characterId} left game world");

                var map = Maps[player.Map];
                map.UnloadPlayer(player);
                player.ClearConnection();
            }
            else
            {
                // 0 means, that connection with client was lost, when he was in character selection screen.
                if (characterId != 0)
                {
                    _logger.LogError($"Couldn't remove player {characterId} from game world");
                }
            }

        }

        /// <inheritdoc />
        public void PlayerSendMotion(int characterId, Motion motion)
        {
            var player = Players[characterId];
            Maps[player.Map].PlayerSendMotion(characterId, motion);
        }

        /// <inheritdoc />
        public async Task PlayerUsedSkill(int characterId, byte skillNumber)
        {
            var player = Players[characterId];
            await player.UseSkill(skillNumber);
        }

        #endregion

        #region Mobs

        /// <inheritdoc />
        public void GMCreateMob(int characterId, ushort mobId)
        {
            var player = Players[characterId];
            if (!player.IsAdmin)
            {
                return;
            }

            // TODO: this should be part of map implementation.
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var mob = Mob.FromDbMob(database.Mobs.First(m => m.Id == mobId), DependencyContainer.Instance.Resolve<ILogger<Mob>>());

            // TODO: mobs should be generated near character, not on his position directly.
            mob.PosX = player.PosX;
            mob.PosY = player.PosY;
            mob.PosZ = player.PosZ;

            Maps[player.Map].AddMob(mob);
        }

        /// <inheritdoc />
        public Mob GetMob(int characterId, int mobId)
        {
            var player = Players[characterId];
            var map = Maps[player.Map];
            var mob = map.GetMob(mobId);
            return mob;
        }

        #endregion
    }
}
