using Imgeneus.Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public string GuildName { get; private set; }


        /// <summary>
        /// Sends list of guilds, right after selection screen.
        /// </summary>
        public void SendGuildList()
        {
            var guilds = _guildManager.GetAllGuilds();

            _packetsHelper.SendGuildList(Client, guilds);
        }
    }
}
