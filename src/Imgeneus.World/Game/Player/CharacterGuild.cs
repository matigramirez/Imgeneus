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
        public string GuildName { get; private set; } = string.Empty;

        /// <summary>
        /// Sends list of guilds, right after selection screen.
        /// </summary>
        public void SendGuildList()
        {
            var guilds = _guildManager.GetAllGuilds(Country);
            _packetsHelper.SendGuildList(Client, guilds);
        }

        /// <summary>
        /// Sends guild members right after selection screen.
        /// </summary>
        public void SendGuildMembers(IEnumerable<DbCharacter> members)
        {
            var online = new List<DbCharacter>();
            var notOnline = new List<DbCharacter>();

            foreach (var m in members)
            {
                if (_gameWorld.Players.ContainsKey(m.Id) || m.Id == Id)
                    online.Add(m);
                else
                    notOnline.Add(m);
            }

            _packetsHelper.SendGuildMembersOnline(Client, online, true);
            _packetsHelper.SendGuildMembersOnline(Client, notOnline, false);
        }
    }
}
