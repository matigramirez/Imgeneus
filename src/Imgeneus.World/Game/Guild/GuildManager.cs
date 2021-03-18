using Imgeneus.World.Game.PartyAndRaid;
using Imgeneus.World.Game.Player;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Imgeneus.World.Game.Guild
{
    public class GuildManager : IGuildManager
    {
        private readonly ILogger<IGuildManager> _logger;

        public const uint MIN_GOLD = 10000000; // 10kk
        public const byte MIN_MEMBERS_COUNT = 7;
        public const byte MIN_LEVEL = 10;

        public GuildManager(ILogger<IGuildManager> logger)
        {
            _logger = logger;
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

                if (guildCreator.Party.Members.All(x => !x.HasGuild))
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
            var request = new GuildCreateRequest(guildCreator, guildCreator.Party.Members);
            var success = CreationRequests.TryAdd(guildCreator.Party, request);

            if (!success)
                guildCreator.SendGuildCreateFailed(GuildCreateFailedReason.Unknown);

            guildCreator.Party.OnMemberEnter += Party_OnMemberChange;
            guildCreator.Party.OnMemberLeft += Party_OnMemberChange;

            foreach (var member in guildCreator.Party.Members)
            {
                if (member == guildCreator)
                    continue;

                member.SendGuildCreateRequest(guildCreator.Id, guildName, guildMessage);
            }
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
        public void SetAgreeRequest(Character character, bool agree)
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
                request.GuildCreator.SendGuildCreateFailed(GuildCreateFailedReason.PartyMemberRejected);
                request.Dispose();
                return;
            }

            var allAgree = request.Acceptance.Values.All(x => x == true);
            if (!allAgree)
                return;

            // TODO: handle call to db for guild creation!

            CreationRequests.TryRemove(character.Party, out request);
            request.GuildCreator.SendGuildCreateFailed(GuildCreateFailedReason.Success);
            request.Dispose();
        }
    }
}
