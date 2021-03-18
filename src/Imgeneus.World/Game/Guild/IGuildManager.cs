using Imgeneus.World.Game.Player;

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
    }
}
