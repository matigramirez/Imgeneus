using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        }

        #region Global id

        private static uint _currentGlobalId;
        private readonly object _currentGlobalIdMutex = new object();

        /// <summary>
        /// Each object in game has its' own global id.
        /// Call this method, when you need to get new global id.
        /// </summary>
        private uint GenerateGlobalId()
        {
            lock (_currentGlobalIdMutex)
            {
                _currentGlobalId++;
            }
            return _currentGlobalId;
        }

        #endregion

        #region Players

        /// <inheritdoc />
        public event Action<Character> OnPlayerEnteredMap;

        /// <inheritdoc />
        public event Action<Character> OnPlayerLeftMap;

        /// <inheritdoc />
        public event Action<Character> OnPlayerMove;

        /// <inheritdoc />
        public event Action<int, Motion> OnPlayerMotion;

        /// <inheritdoc />
        public event Action<Character, Skill> OnPlayerUsedSkill;

        /// <inheritdoc />
        public event Action<Character, ActiveBuff> OnPlayerGotBuff;

        /// <inheritdoc />
        public ConcurrentDictionary<int, Character> Players { get; private set; } = new ConcurrentDictionary<int, Character>();

        /// <inheritdoc />
        public Character LoadPlayer(int characterId)
        {
            using var database = DependencyContainer.Instance.Resolve<IDatabase>();
            var dbCharacter = database.Characters.Include(c => c.Skills).ThenInclude(cs => cs.Skill)
                                               .Include(c => c.Items).ThenInclude(ci => ci.Item)
                                               .Include(c => c.ActiveBuffs).ThenInclude(cb => cb.Skill)
                                               .Include(c => c.User)
                                               .FirstOrDefault(c => c.Id == characterId);
            var newPlayer = Character.FromDbCharacter(dbCharacter, DependencyContainer.Instance.Resolve<ILogger<Character>>());

            Players.TryAdd(newPlayer.Id, newPlayer);
            _logger.LogDebug($"Player {newPlayer.Id} connected to game world");

            return newPlayer;
        }

        /// <inheritdoc />
        public async Task PlayerMoves(int characterId, MovementType movementType, float X, float Y, float Z, ushort angle)
        {
            var player = Players[characterId];
            player.PosX = X;
            player.PosY = Y;
            player.PosZ = Z;
            player.Angle = angle;
            OnPlayerMove?.Invoke(player);

            if (movementType == MovementType.Stopped)
            {
                using var database = DependencyContainer.Instance.Resolve<IDatabase>();
                var dbCharacter = database.Characters.Find(characterId);
                dbCharacter.Angle = angle;
                dbCharacter.PosX = X;
                dbCharacter.PosY = Y;
                dbCharacter.PosZ = Z;
                await database.SaveChangesAsync();
            }

            _logger.LogDebug($"Character {player.Id} moved to x={player.PosX} y={player.PosY} z={player.PosZ} angle={player.Angle}");
        }

        /// <inheritdoc />
        public Character LoadPlayerInMap(int characterId)
        {
            var player = Players[characterId];

            // TODO: implement maps. For now just notify other players, that new player arrived.

            OnPlayerEnteredMap?.Invoke(player);

            return player;
        }

        /// <inheritdoc />
        public void RemovePlayer(int characterId)
        {
            Character player;
            if (Players.TryRemove(characterId, out player))
            {
                OnPlayerLeftMap?.Invoke(player);
                _logger.LogDebug($"Player {characterId} left game world");
            }
            else
            {
                _logger.LogError($"Couldn't remove player {characterId} from game world");
            }

        }

        /// <inheritdoc />
        public void PlayerSendMotion(int characterId, Motion motion)
        {
            if (motion == Motion.None || motion == Motion.Sit)
            {
                var player = Players[characterId];
                player.Motion = motion;
            }
            OnPlayerMotion?.Invoke(characterId, motion);
        }

        /// <inheritdoc />
        public async Task PlayerUsedSkill(int characterId, byte skillNumber)
        {
            var player = Players[characterId];
            var skill = player.Skills.First(s => s.Number == skillNumber);

            // TODO: implement use of all skills.
            // For now, just for testing I'm implementing buff to character.
            if (skill.Type == TypeDetail.Buff && (skill.TargetType == TargetType.Caster || skill.TargetType == TargetType.PartyMembers))
            {
                var buff = await player.AddActiveBuff(skill);
                OnPlayerGotBuff?.Invoke(player, buff);
            }

            OnPlayerUsedSkill?.Invoke(player, skill);
        }

        #endregion

        #region Mobs

        public List<Mob> Mobs = new List<Mob>();

        /// <inheritdoc />
        public event Action<Mob> OnMobEnter;

        /// <inheritdoc />
        public event Action<Mob> OnMobMove;

        /// <inheritdoc />
        public event Action<Mob, int> OnMobAttack;

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
            var mob = Mob.FromDbMob(GenerateGlobalId(), database.Mobs.First(m => m.Id == mobId), DependencyContainer.Instance.Resolve<ILogger<Mob>>());

            // TODO: mobs should be generated near character, not on his position directly.
            mob.PosX = player.PosX;
            mob.PosY = player.PosY;
            mob.PosZ = player.PosZ;

            Mobs.Add(mob);
            _logger.LogDebug($"Mob {mob.MobId} entered game world");
            OnMobEnter?.Invoke(mob);

            // TODO: I'm investigating all available mob packets now.
            // Remove it, when start working on AI implementation!

            // Emulates mob move within 3 seconds after it's created.
            //mob.OnMove += (sender) =>
            //{
            //    OnMobMove?.Invoke(sender);
            //};
            //mob.EmulateMovement();

            // Emulates mob attack within 3 seconds after it's created.
            //mob.OnAttack += (mob, playerId) =>
            //{
            //    OnMobAttack?.Invoke(mob, playerId);
            //};
            //mob.EmulateAttack();
        }

        /// <inheritdoc />
        public Mob GetMob(int characterId, uint mobId)
        {
            var mob = Mobs.FirstOrDefault(m => m.GlobalId == mobId);
            return mob;
        }

        #endregion
    }
}
