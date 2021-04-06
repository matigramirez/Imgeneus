using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Imgeneus.World.Game.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public class GuildManager : IGuildManager
    {
        private readonly ILogger<IGuildManager> _logger;
        private readonly IDatabase _database;
        private readonly IGameWorld _gameWorld;
        private readonly ITimeService _timeService;
        private SemaphoreSlim _sync = new SemaphoreSlim(1);

        private readonly IGuildConfiguration _config;
        private readonly IGuildHouseConfiguration _houseConfig;

        public GuildManager(ILogger<IGuildManager> logger, IGuildConfiguration config, IGuildHouseConfiguration houseConfig, IDatabase database, IGameWorld gameWorld, ITimeService timeService)
        {
            _logger = logger;
            _database = database;
            _gameWorld = gameWorld;
            _timeService = timeService;

            _config = config;
            _houseConfig = houseConfig;
        }

        #region Guild creation

        /// <inheritdoc/>
        public async Task<GuildCreateFailedReason> CanCreateGuild(Character guildCreator, string guildName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(guildName))
                    return GuildCreateFailedReason.WrongName;

                if (guildCreator.Gold < _config.MinGold)
                    return GuildCreateFailedReason.NotEnoughGold;

                if (!guildCreator.HasParty || !(guildCreator.Party is Party) || guildCreator.Party.Members.Count != _config.MinMembers)
                    return GuildCreateFailedReason.NotEnoughMembers;

                if (!guildCreator.Party.Members.All(x => x.Level > _config.MinLevel))
                    return GuildCreateFailedReason.LevelLimit;

                // TODO: banned words?
                // if(guildName.Contains(bannedWords))
                // return GuildCreateFailedReason.WrongName;

                if (guildCreator.Party.Members.Any(x => x.HasGuild))
                    return GuildCreateFailedReason.PartyMemberInAnotherGuild;

                var penalty = false;
                foreach (var m in guildCreator.Party.Members)
                {
                    if (await CheckPenalty(m.Id))
                    {
                        penalty = true;
                        break;
                    }
                }
                if (penalty)
                    return GuildCreateFailedReason.PartyMemberGuildPenalty;

                return GuildCreateFailedReason.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return GuildCreateFailedReason.Unknown;
            }
        }

        /// <summary>
        /// Ensures, that character doesn't have a penalty.
        /// </summary>
        /// <returns>true is penalty</returns>
        private async Task<bool> CheckPenalty(int characterId)
        {
            var character = await _database.Characters.FindAsync(characterId);
            if (character is null)
                return true;

            if (character.GuildLeaveTime is null)
                return false;

            var leaveTime = (DateTime)character.GuildLeaveTime;
            return _timeService.UtcNow.Subtract(leaveTime).TotalHours < _config.MinPenalty;
        }

        /// <summary>
        /// Guild creation requests.
        /// </summary>
        public static ConcurrentDictionary<IParty, GuildCreateRequest> CreationRequests { get; private set; } = new ConcurrentDictionary<IParty, GuildCreateRequest>();

        /// <inheritdoc/>
        public void SendGuildRequest(Character guildCreator, string guildName, string guildMessage)
        {
            var request = new GuildCreateRequest(guildCreator, guildCreator.Party.Members, guildName, guildMessage);
            var success = CreationRequests.TryAdd(guildCreator.Party, request);

            if (!success)
                guildCreator.SendGuildCreateFailed(GuildCreateFailedReason.Unknown);

            guildCreator.Party.OnMemberEnter += Party_OnMemberChange;
            guildCreator.Party.OnMemberLeft += Party_OnMemberChange;

            foreach (var member in guildCreator.Party.Members)
                member.SendGuildCreateRequest(guildCreator.Id, guildName, guildMessage);
        }

        private void Party_OnMemberChange(IParty party)
        {
            CreationRequests.TryRemove(party, out var creationRequest);

            if (creationRequest != null)
            {
                creationRequest.GuildCreator.SendGuildCreateFailed(GuildCreateFailedReason.Unknown);
                creationRequest.Dispose();
            }

            party.OnMemberEnter -= Party_OnMemberChange;
            party.OnMemberLeft -= Party_OnMemberChange;
        }

        /// <inheritdoc/>
        public async void SetAgreeRequest(Character character, bool agree)
        {
            if (character.Party is null)
                return;

            CreationRequests.TryGetValue(character.Party, out var request);

            if (request is null)
                return;

            if (!request.Acceptance.ContainsKey(character.Id))
                return;

            request.Acceptance[character.Id] = agree;

            if (!agree)
            {
                CreationRequests.TryRemove(character.Party, out request);
                foreach (var m in request.Members)
                    m.SendGuildCreateFailed(GuildCreateFailedReason.PartyMemberRejected);
                request.Dispose();
                return;
            }

            var allAgree = request.Acceptance.Values.All(x => x == true);
            if (!allAgree)
                return;

            CreationRequests.TryRemove(character.Party, out request);

            if (request is null) // is it possible?
            {
                return;
            }

            var guild = await TryCreateGuild(request.Name, request.Message, request.GuildCreator);
            if (guild is null) // Creation failed.
            {
                foreach (var m in request.Members)
                    m.SendGuildCreateFailed(GuildCreateFailedReason.Unknown);
                request.Dispose();
                return;
            }

            foreach (var m in request.Members)
            {
                byte rank = 9;
                if (m == request.GuildCreator)
                    rank = 1;

                await TryAddMember(guild.Id, m.Id, rank);

                m.GuildId = guild.Id;
                m.GuildName = guild.Name;
                m.GuildRank = rank;
            }

            foreach (var m in request.Members)
            {
                m.GuildMembers.AddRange(guild.Members);
                m.SendGuildMembersOnline();
                m.SendGuildCreateSuccess(guild.Id, m.GuildRank, guild.Name, request.Message);
            }

            request.GuildCreator.ChangeGold(request.GuildCreator.Gold - _config.MinGold);
            request.GuildCreator.SendGoldUpdate();

            foreach (var player in _gameWorld.Players.Values.ToList())
                player.SendGuildListAdd(guild);

            request.Dispose();
        }

        /// <summary>
        /// Creates guild in database.
        /// </summary>
        /// <returns>Db guild, if it was created, otherwise null.</returns>
        private async Task<DbGuild> TryCreateGuild(string name, string message, Character master)
        {
            await _sync.WaitAsync();

            var result = await CreateGuild(name, message, master);

            _sync.Release();

            return result;
        }

        private async Task<DbGuild> CreateGuild(string name, string message, Character master)
        {
            var guild = new DbGuild(name, message, master.Id, master.Country);

            _database.Guilds.Add(guild);

            var result = await _database.SaveChangesAsync();

            if (result > 0)
                return guild;
            else
                return null;
        }

        #endregion

        #region Guild remove

        /// <inheritdoc/>
        public async Task<bool> TryDeleteGuild(int guildId)
        {
            await _sync.WaitAsync();

            var result = await DeleteGuild(guildId);

            _sync.Release();

            return result;
        }

        private async Task<bool> DeleteGuild(int guildId)
        {
            var guild = await _database.Guilds.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == guildId);
            if (guild is null)
                return false;

            foreach (var m in guild.Members)
                m.GuildRank = 0;

            _database.Guilds.Remove(guild);

            var result = await _database.SaveChangesAsync();
            var success = result > 0;

            if (success)
            {
                foreach (var player in _gameWorld.Players.Values.ToList())
                    player.SendGuildListRemove(guildId);
            }

            return success;
        }

        #endregion

        #region Add/remove members

        /// <inheritdoc/>
        public async Task<DbCharacter> TryAddMember(int guildId, int characterId, byte rank = 9)
        {
            await _sync.WaitAsync();

            var result = await AddMember(guildId, characterId, rank);

            _sync.Release();

            return result;
        }

        private async Task<DbCharacter> AddMember(int guildId, int characterId, byte rank = 9)
        {
            var guild = await _database.Guilds.FindAsync(guildId);
            if (guild is null)
                return null;

            var character = await _database.Characters.FindAsync(characterId);
            if (character is null)
                return null;

            guild.Members.Add(character);
            character.GuildRank = rank;
            character.GuildJoinTime = _timeService.UtcNow;

            var result = await _database.SaveChangesAsync();
            if (result > 0)
                return character;
            else
                return null;
        }

        /// <inheritdoc/>
        public async Task<DbCharacter> TryRemoveMember(int guildId, int characterId)
        {
            await _sync.WaitAsync();

            var result = await RemoveMember(guildId, characterId);

            _sync.Release();

            return result;

        }

        private async Task<DbCharacter> RemoveMember(int guildId, int characterId)
        {
            var guild = await _database.Guilds.FindAsync(guildId);
            if (guild is null)
                return null;

            var character = await _database.Characters.FindAsync(characterId);
            if (character is null)
                return null;

            guild.Members.Remove(character);
            character.GuildRank = 0;
            character.GuildLeaveTime = _timeService.UtcNow;

            var result = await _database.SaveChangesAsync();
            if (result > 0)
                return character;
            else
                return null;
        }


        #endregion

        #region List guilds

        /// <inheritdoc/>
        public DbGuild[] GetAllGuilds(Fraction country = Fraction.NotSelected)
        {
            if (country == Fraction.NotSelected)
                return _database.Guilds.Include(g => g.Master).ToArray();

            return _database.Guilds.Include(g => g.Master).Where(g => g.Country == country).ToArray();
        }

        /// <inheritdoc/>
        public async Task<DbGuild> GetGuild(int guildId)
        {
            return await _database.Guilds.FindAsync(guildId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DbCharacter>> GetMemebers(int guildId)
        {
            var guild = await _database.Guilds.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == guildId);
            if (guild is null)
                return new List<DbCharacter>();

            return guild.Members;
        }

        /// <inheritdoc/>
        public void ReloadGuildRanks(IEnumerable<(int GuildId, int Points, byte Rank)> results)
        {
            foreach (var res in results)
            {
                var guild = _database.Guilds.Find(res.GuildId);
                if (guild is null)
                    return;

                guild.Points = res.Points;
                guild.Rank = res.Rank;
            }

            // Likely no need to save to db since GuildRankingManager will enqueue save?
        }

        #endregion

        #region Request join

        /// <summary>
        /// Dictionary of join requests.
        /// Key is player id.
        /// Value is guild id.
        /// </summary>
        public static readonly ConcurrentDictionary<int, int> JoinRequests = new ConcurrentDictionary<int, int>();

        /// <inheritdoc/>
        public async Task<bool> RequestJoin(int guildId, int playerId)
        {
            var guild = await _database.Guilds.FindAsync(guildId);
            if (guild is null)
                return false;

            var character = await _database.Characters.FindAsync(playerId);
            if (character is null)
                return false;

            await RemoveRequestJoin(playerId);

            JoinRequests.TryAdd(playerId, guildId);

            foreach (var m in guild.Members.Where(x => x.GuildRank < 3))
            {
                if (!_gameWorld.Players.ContainsKey(m.Id))
                    continue;

                var guildMember = _gameWorld.Players[m.Id];
                guildMember.SendGuildJoinRequestAdd(character);
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task RemoveRequestJoin(int playerId)
        {
            if (JoinRequests.TryRemove(playerId, out var removed))
            {
                var guild = await _database.Guilds.FindAsync(removed);
                if (guild is null)
                    return;

                foreach (var m in guild.Members.Where(x => x.GuildRank < 3))
                {
                    if (!_gameWorld.Players.ContainsKey(m.Id))
                        continue;

                    var guildMember = _gameWorld.Players[m.Id];
                    guildMember.SendGuildJoinRequestRemove(playerId);
                }
            }
        }

        #endregion

        #region Member rank change

        /// <inheritdoc/>
        public async Task<DbCharacter> TryChangeRank(int guildId, int playerId, bool demote)
        {
            await _sync.WaitAsync();

            var result = await ChangeRank(guildId, playerId, demote);

            _sync.Release();

            return result;

        }

        private async Task<DbCharacter> ChangeRank(int guildId, int playerId, bool demote)
        {
            var character = await _database.Characters.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Id == playerId);
            if (character is null)
                return null;

            if (demote && character.GuildRank == 9)
                return null;

            if (!demote && character.GuildRank == 2)
                return null;

            if (demote)
                character.GuildRank++;
            else
                character.GuildRank--;

            var result = await _database.SaveChangesAsync();

            if (result > 0)
                return character;
            else
                return null;
        }

        #endregion

        #region Guild house

        /// <inheritdoc/>
        public async Task<GuildHouseBuyReason> TryBuyHouse(Character character)
        {
            await _sync.WaitAsync();

            var result = await BuyHouse(character);

            _sync.Release();

            return result;
        }

        private async Task<GuildHouseBuyReason> BuyHouse(Character character)
        {
            if (character.GuildRank != 1)
            {
                return GuildHouseBuyReason.NotAuthorized;
            }

            if (character.Gold < _houseConfig.HouseBuyMoney)
            {
                return GuildHouseBuyReason.NoGold;
            }

            var guild = await GetGuild((int)character.GuildId);
            if (guild is null || guild.Rank > 30)
            {
                return GuildHouseBuyReason.LowRank;
            }

            if (guild.HasHouse)
            {
                return GuildHouseBuyReason.AlreadyBought;
            }

            character.ChangeGold((uint)(character.Gold - _houseConfig.HouseBuyMoney));

            guild.HasHouse = true;
            await _database.SaveChangesAsync();

            return GuildHouseBuyReason.Ok;
        }

        ///  <inheritdoc/>
        public bool HasHouse(int guildId)
        {
            var guild = _database.Guilds.Find(guildId);
            if (guild is null)
                return false;

            return guild.HasHouse;
        }

        ///  <inheritdoc/>
        public byte GetRank(int guildId)
        {
            var guild = _database.Guilds.Find(guildId);
            if (guild is null)
                return 0;

            return guild.Rank;
        }

        ///  <inheritdoc/>
        public bool CanUseNpc(int guildId, byte type, ushort typeId, out byte requiredRank)
        {
            requiredRank = 30;

            var guild = _database.Guilds.Find(guildId);
            if (guild is null || guild.Rank > 30)
                return false;

            GuildHouseNpcInfo npcInfo;
            if (guild.Country == Fraction.Light)
            {
                npcInfo = _houseConfig.NpcInfos.FirstOrDefault(x => x.NpcType == type && x.LightNpcTypeId == typeId);
            }
            else
            {
                npcInfo = _houseConfig.NpcInfos.FirstOrDefault(x => x.NpcType == type && x.DarkNpcTypeId == typeId);
            }

            if (npcInfo is null)
                return false;

            requiredRank = npcInfo.MinRank;
            return requiredRank >= guild.Rank;
        }

        ///  <inheritdoc/>
        public bool HasNpcLevel(int guildId, byte type, ushort typeId)
        {
            var guild = _database.Guilds.Include(x => x.NpcLvls).FirstOrDefault(x => x.Id == guildId);
            if (guild is null)
                return false;

            GuildHouseNpcInfo npcInfo;
            if (guild.Country == Fraction.Light)
            {
                npcInfo = _houseConfig.NpcInfos.FirstOrDefault(x => x.NpcType == type && x.LightNpcTypeId == typeId);
            }
            else
            {
                npcInfo = _houseConfig.NpcInfos.FirstOrDefault(x => x.NpcType == type && x.DarkNpcTypeId == typeId);
            }

            if (npcInfo is null)
                return false;

            if (npcInfo.NpcLvl == 0)
                return true;

            var currentLevel = guild.NpcLvls.FirstOrDefault(x => x.NpcType == npcInfo.NpcType && x.Group == npcInfo.Group);

            return currentLevel != null && currentLevel.NpcLevel >= npcInfo.NpcLvl;
        }

        #endregion

        #region Etin

        public async Task<int> GetEtin(int guildId)
        {
            var guild = await GetGuild(guildId);
            return guild.Etin;
        }

        #endregion
    }
}
