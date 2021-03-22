using Imgeneus.Database;
using Imgeneus.Database.Entities;
using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public class GuildManager : IGuildManager
    {
        private readonly ILogger<IGuildManager> _logger;
        private readonly IDatabase _database;

        public const uint MIN_GOLD = 10000000; // 10kk
        public const byte MIN_MEMBERS_COUNT = 2;//7;
        public const byte MIN_LEVEL = 10;

        public GuildManager(ILogger<IGuildManager> logger, IDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        /// <inheritdoc/>
        public GuildCreateFailedReason CanCreateGuild(Character guildCreator, string guildName)
        {
            try
            {
                if (guildCreator.Gold < MIN_GOLD)
                    return GuildCreateFailedReason.NotEnoughGold;

                if (!guildCreator.HasParty || !(guildCreator.Party is Party) || guildCreator.Party.Members.Count != MIN_MEMBERS_COUNT)
                    return GuildCreateFailedReason.NotEnoughMembers;

                if (!guildCreator.Party.Members.All(x => x.Level > MIN_LEVEL))
                    return GuildCreateFailedReason.LevelLimit;

                // TODO: banned words?
                // if(guildName.Contains(bannedWords))
                // return GuildCreateFailedReason.WrongName;

                if (guildCreator.Party.Members.Any(x => x.HasGuild))
                    return GuildCreateFailedReason.PartyMemberInAnotherGuild;

                return GuildCreateFailedReason.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return GuildCreateFailedReason.Unknown;
            }
        }

        /// <summary>
        /// Guild creation requests.
        /// </summary>
        public ConcurrentDictionary<IParty, GuildCreateRequest> CreationRequests { get; private set; } = new ConcurrentDictionary<IParty, GuildCreateRequest>();

        /// <inheritdoc/>
        public void SendGuildRequest(Character guildCreator, string guildName, string guildMessage)
        {
            var request = new GuildCreateRequest(guildCreator, guildCreator.Party.Members.ToList(), guildName, guildMessage);
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

                await TryAddMember(guild.Id, m, rank);
                m.SendGuildCreateSuccess(guild.Id, rank, guild.Name, request.Message);
            }

            request.GuildCreator.ChangeGold(request.GuildCreator.Gold - MIN_GOLD);
            request.GuildCreator.SendGoldUpdate();

            // TODO: send GUILD_USER_LIST_ONLINE
            // TODO: send GUILD_LIST_ADD

            request.Dispose();
        }

        /// <summary>
        /// Creates guild in database.
        /// </summary>
        /// <returns>Db guild, if it was created, otherwise null.</returns>
        private async Task<DbGuild> TryCreateGuild(string name, string message, Character master)
        {
            var guild = new DbGuild(name, message, master.Id, master.Country);

            _database.Guilds.Add(guild);

            var result = await _database.SaveChangesAsync();
            if (result > 0)
                return guild;
            else
                return null;
        }

        /// <inheritdoc/>
        public async Task<bool> TryAddMember(int guildId, Character member, byte rank)
        {
            var guild = await _database.Guilds.FindAsync(guildId);
            if (guild is null)
                return false;

            var character = await _database.Characters.FindAsync(member.Id);
            if (character is null)
                return false;

            guild.Members.Add(character);
            character.GuildRank = rank;

            var result = await _database.SaveChangesAsync();
            if (result > 0)
                return true;
            else
                return false;
        }

        /// <inheritdoc/>
        public DbGuild[] GetAllGuilds(Fraction country = Fraction.NotSelected)
        {
            if (country == Fraction.NotSelected)
                return _database.Guilds.Include(g => g.Master).ToArray();

            return _database.Guilds.Include(g => g.Master).Where(g => g.Country == country).ToArray();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DbCharacter>> GetMemebers(int guildId)
        {
            var guild = await _database.Guilds.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == guildId);
            if (guild is null)
                return new List<DbCharacter>();

            return guild.Members;
        }
    }
}
