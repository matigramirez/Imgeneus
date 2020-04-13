using Imgeneus.Core.DependencyInjection;
using Imgeneus.Database;
using Imgeneus.Database.Constants;
using Imgeneus.Network.Packets.Game;
using Imgeneus.World.Game.Monster;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Packets;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game.Zone
{
    /// <summary>
    /// Zone, where users, mobs, npc are presented.
    /// </summary>
    public class Map
    {
        #region Constructor

        private readonly ILogger<Map> _logger;
        private readonly MapPacketsHelper _packetHelper;

        /// <summary>
        /// Map id.
        /// </summary>
        public ushort Id { get; private set; }

        public Map(ushort id, ILogger<Map> logger)
        {
            Id = id;
            _logger = logger;
            _packetHelper = new MapPacketsHelper();
        }

        #endregion

        #region Players

        /// <summary>
        /// Thread-safe dictionary of connected players. Key is character id, value is character.
        /// </summary>
        private readonly ConcurrentDictionary<int, Character> Players = new ConcurrentDictionary<int, Character>();

        /// <summary>
        /// Loads player into map.
        /// </summary>
        /// <param name="character">player, that we need to load</param>
        /// <returns>returns true if we could load player to map, otherwise false</returns>
        public bool LoadPlayer(Character character)
        {
            var success = Players.TryAdd(character.Id, character);

            if (success)
            {
                _logger.LogDebug($"Player {character.Id} connected to map {Id}");

                foreach (var loadedPlayer in Players)
                {
                    if (loadedPlayer.Key != character.Id)
                    {
                        // Notify players in this map, that new player arrived.
                        _packetHelper.SendCharacterConnectedToMap(loadedPlayer.Value.Client, character);

                        // Notify new player, about already loaded player.
                        _packetHelper.SendCharacterConnectedToMap(character.Client, loadedPlayer.Value);
                    }
                }

                character.OnUsedSkill += Character_OnUsedSkill;
                character.OnPositionChanged += Character_OnPositionChanged;
                character.OnMotion += Character_OnMotion;
                character.OnSeekForTarget += Character_OnTargetChanged;

                if (character.IsAdmin)
                {
                    character.Client.OnPacketArrived += Client_OnPacketArrived;
                }
            }

            return success;
        }

        /// <summary>
        /// Unloads player from map.
        /// </summary>
        /// <param name="character">player, that we need to unload</param>
        /// <returns>returns true if we could unload player to map, otherwise false</returns>
        public bool UnloadPlayer(Character character)
        {
            var success = Players.TryRemove(character.Id, out var removedCharacter);

            if (success)
            {
                _logger.LogDebug($"Player {character.Id} left map {Id}");

                // Send other clients notification, that user has left the map.
                foreach (var player in Players)
                {
                    _packetHelper.SendCharacterLeftMap(player.Value.Client, removedCharacter);
                }

                character.OnUsedSkill -= Character_OnUsedSkill;
                character.OnPositionChanged -= Character_OnPositionChanged;
            }

            return success;
        }

        /// <summary>
        /// Notifies other players about position change.
        /// </summary>
        private void Character_OnPositionChanged(Character movedPlayer)
        {
            // Send other clients notification, that user is moving.
            foreach (var player in Players)
            {
                if (player.Key != movedPlayer.Id)
                {
                    _packetHelper.SendCharacterMoves(player.Value.Client, movedPlayer);
                }
            }
        }

        /// <summary>
        /// When player sends motion, we should resend this motion to all other players on this map.
        /// </summary>
        private void Character_OnMotion(Character playerWithMotion, Motion motion)
        {
            // Notify all players about new motion.
            foreach (var player in Players)
            {
                _packetHelper.SendCharacterMotion(player.Value.Client, playerWithMotion.Id, motion);
            }
        }

        /// <summary>
        /// Notifies other players, that this player used some skill.
        /// </summary>
        /// <param name="sender">player, that used skill</param>
        /// <param name="skill">skill, that was used</param>
        private void Character_OnUsedSkill(Character sender, Skill skill)
        {
            // Notify all players about used skill.
            foreach (var player in Players)
            {
                // Just plays skill animation.
                _packetHelper.SendCharacterUsedSkilll(player.Value.Client, sender, sender, skill);
            }
        }

        /// <summary>
        /// Sets target based on what target character wants to get.
        /// </summary>
        /// <param name="sender">character, that seeks for target</param>
        /// <param name="targetId">target Id</param>
        /// <param name="targetType">mob or another player</param>
        private void Character_OnTargetChanged(Character sender, int targetId, TargetEntity targetType)
        {
            if (targetType == TargetEntity.Mob)
            {
                sender.Target = Mobs[targetId];
            }
            else
            {
                sender.Target = Players[targetId];
            }
        }

        /// <summary>
        /// Handles special packets, as GM packets mob creation etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        private void Client_OnPacketArrived(WorldClient sender, IDeserializedPacket packet)
        {
            switch (packet)
            {
                case GMCreateMobPacket gMCreateMobPacket:
                    // TODO: find out way to preload all awailable mobs.
                    using (var database = DependencyContainer.Instance.Resolve<IDatabase>())
                    {
                        var mob = Mob.FromDbMob(database.Mobs.First(m => m.Id == gMCreateMobPacket.MobId), DependencyContainer.Instance.Resolve<ILogger<Mob>>());

                        var gmPlayer = Players[sender.CharID];
                        // TODO: mobs should be generated near character, not on his position directly.
                        mob.PosX = gmPlayer.PosX;
                        mob.PosY = gmPlayer.PosY;
                        mob.PosZ = gmPlayer.PosZ;

                        AddMob(mob);
                    }
                    break;
            }
        }

        #endregion

        #region Mobs

        private int _currentGlobalMobId;
        private readonly object _currentGlobalMobIdMutex = new object();

        /// <summary>
        /// Each mob in game has its' own id.
        /// Call this method, when you need to get new mob id.
        /// </summary>
        private int GenerateMobId()
        {
            lock (_currentGlobalMobIdMutex)
            {
                _currentGlobalMobId++;
            }
            return _currentGlobalMobId;
        }

        /// <summary>
        /// Thread-safe dictionary of monsters loaded to this map. Where key id mob id.
        /// </summary>
        private readonly ConcurrentDictionary<int, Mob> Mobs = new ConcurrentDictionary<int, Mob>();

        /// <summary>
        /// Tries to add mob to map and notifies other players, that mob arrived.
        /// </summary>
        /// <returns>turue if mob was added, otherwise false</returns>
        public bool AddMob(Mob mob)
        {
            var id = GenerateMobId();
            var success = Mobs.TryAdd(id, mob);
            if (success)
            {
                mob.Id = id;
                _logger.LogDebug($"Mob {mob.MobId} entered game world");

                foreach (var player in Players)
                {
                    _packetHelper.SendMobEntered(player.Value.Client, mob);
                }


                // TODO: I'm investigating all available mob packets now.
                // Remove it, when start working on AI implementation!

                // Emulates mob move within 3 seconds after it's created.
                //mob.OnMove += (sender) =>
                //{
                //    foreach (var player in Players)
                //    {
                //        _packetHelper.SendMobEntered(player.Value.Client, sender);
                //    }
                //};
                //mob.EmulateMovement();

                // Emulates mob attack within 3 seconds after it's created.
                //mob.OnAttack += (mob, playerId) =>
                //{
                //    // Send notification each player, that mob attacked.
                //    foreach (var player in Players)
                //    {
                //        _packetHelper.SendMobAttack(player.Value.Client, mob, playerId);
                //    }
                //};
                //mob.EmulateAttack(Players.First().Key);
            }

            return success;
        }

        /// <summary>
        /// Tries to get mob from map.
        /// </summary>
        /// <param name="mobId">id of mob, that you are trying to get.</param>
        /// <returns>either mob or null if mob is not presented</returns>
        public Mob GetMob(int mobId)
        {
            Mobs.TryGetValue(mobId, out var mob);
            return mob;
        }

        #endregion

    }
}
