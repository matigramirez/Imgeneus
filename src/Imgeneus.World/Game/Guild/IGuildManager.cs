using Imgeneus.Database.Entities;
using Imgeneus.World.Game.Player;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imgeneus.World.Game.Guild
{
    public interface IGuildManager
    {
        /// <summary>
        /// Checks if character can create a guild.
        /// </summary>
        public GuildCreateFailedReason CanCreateGuild(Character guildCreator, string guildName);

        /// <summary>
        /// Sends guild create request to all party members.
        /// </summary>
        public void SendGuildRequest(Character guildCreator, string guildName, string guildMessage);

        /// <summary>
        /// Sets result of request to player.
        /// </summary>
        /// <param name="character">player to whom the request was sent</param>
        /// <param name="agree">player's answer</param>
        public void SetAgreeRequest(Character character, bool agree);

        /// <summary>
        /// Tries to add character to guild.
        /// </summary>
        /// <param name="guildId">guild id</param>
        /// <param name="member">new character</param>
        /// <param name="rank">character rank in guild</param>
        /// <returns>true, if character was added to guild, otherwise false</returns>
        public Task<bool> TryAddMember(int guildId, Character member, byte rank);

        /// <summary>
        /// Get all guilds in this server.
        /// </summary>
        /// <param name="country">optional param, fraction light or dark</param>
        /// <returns>collection of guilds</returns>
        public DbGuild[] GetAllGuilds(Fraction country = Fraction.NotSelected);

        /// <summary>
        /// Gets guild members.
        /// </summary>
        /// <returns>collection of memebers</returns>
        public Task<IEnumerable<DbCharacter>> GetMemebers(int guildId);

        /// <summary>
        /// Player requests to join a guild.
        /// </summary>
        /// <returns>true is success</returns>
        public Task<bool> RequestJoin(int guildId, int playerId);
    }
}
