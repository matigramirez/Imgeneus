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
    }
}
