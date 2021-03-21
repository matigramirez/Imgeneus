using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterGuild")]
    public class DbCharacterGuild
    {
        public int CharacterId { get; set; }

        public int GuildId { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public DbCharacter Character { get; set; }

        [ForeignKey(nameof(GuildId))]
        public DbGuild Guild { get; set; }

        /// <summary>
        /// Character level in guild hierarchy.
        /// </summary>
        public byte GuildLevel { get; set; }

        /// <summary>
        /// Date, when character joined guild.
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// Date, when character left guild.
        /// </summary>
        public DateTime LeaveDate { get; set; }

        public DbCharacterGuild(int characterId, int guildId, byte guildLevel)
        {
            CharacterId = characterId;
            GuildId = guildId;
            GuildLevel = guildLevel;
            JoinDate = DateTime.UtcNow;
        }
    }
}
