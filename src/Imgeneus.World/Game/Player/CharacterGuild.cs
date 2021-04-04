using Imgeneus.Database.Entities;
using System.Collections.Generic;

namespace Imgeneus.World.Game.Player
{
    public partial class Character
    {
        /// <summary>
        /// Guild id.
        /// </summary>
        public int? GuildId { get; set; }

        /// <summary>
        /// Bool indicator, that shows if character is guild member.
        /// </summary>
        public bool HasGuild { get => GuildId != null; }

        /// <summary>
        /// Guild name.
        /// </summary>
        public string GuildName { get; set; } = string.Empty;

        /// <summary>
        /// Guild rank.
        /// </summary>
        public byte GuildRank { get; set; }

        /// <summary>
        /// Guild member ids for easy notification.
        /// </summary>
        public readonly List<DbCharacter> GuildMembers = new List<DbCharacter>();

        /// <summary>
        /// Sends list of guilds, right after selection screen.
        /// </summary>
        public void SendGuildList()
        {
            var guilds = _guildManager.GetAllGuilds(Country);
            _packetsHelper.SendGuildList(Client, guilds);
        }

        public void SendGuildMembersOnline()
        {
            var online = new List<DbCharacter>();
            var notOnline = new List<DbCharacter>();

            foreach (var m in GuildMembers)
            {
                if (_gameWorld.Players.ContainsKey(m.Id) || m.Id == Id)
                    online.Add(m);
                else
                    notOnline.Add(m);
            }

            _packetsHelper.SendGuildMembersOnline(Client, online, true);
            _packetsHelper.SendGuildMembersOnline(Client, notOnline, false);
        }

        /// <summary>
        /// Notifies guild members, that player is online.
        /// </summary>
        public void NotifyGuildMembersOnline()
        {
            foreach (var m in GuildMembers)
            {
                var id = m.Id;
                if (id == Id)
                    continue;

                if (!_gameWorld.Players.ContainsKey(id))
                    continue;

                _gameWorld.Players.TryGetValue(id, out var player);

                if (player is null)
                    continue;

                player.SendGuildMemberIsOnline(Id);
            }
        }

        /// <summary>
        /// Notifies guild members, that player is offline.
        /// </summary>
        public void NotifyGuildMembersOffline()
        {
            foreach (var m in GuildMembers)
            {
                var id = m.Id;
                if (id == Id)
                    continue;

                if (!_gameWorld.Players.ContainsKey(id))
                    continue;

                _gameWorld.Players.TryGetValue(id, out var player);

                if (player is null)
                    continue;

                player.SendGuildMemberIsOffline(Id);
            }
        }

        /// <summary>
        /// Clears guild values from player. Not from DB!
        /// </summary>
        public void ClearGuild()
        {
            GuildId = null;
            GuildName = string.Empty;
            GuildRank = 0;
            GuildMembers.Clear();
        }

        public bool GuildHasHouse
        {
            get
            {
                if (!HasGuild)
                    return false;

                return _guildManager.HasHouse((int)GuildId);
            }
        }

        public bool GuildHasTopRank
        {
            get
            {
                if (!HasGuild)
                    return false;

                return _guildManager.GetRank((int)GuildId) <= 30;
            }
        }

        /// <summary>
        /// Reloads guild ranks for <see cref="_guildManager"/>.
        /// </summary>
        public void ReloadGuildRanks(IEnumerable<(int guildId, int points, byte rank)> results)
        {
            _guildManager.ReloadGuildRanks(results);
        }
    }
}
