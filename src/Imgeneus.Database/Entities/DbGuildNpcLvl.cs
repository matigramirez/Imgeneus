using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("GuildNpcLvl")]
    public class DbGuildNpcLvl
    {
        /// <summary>
        /// Gets the guild associated id.
        /// </summary>
        [Required]
        public int GuildId { get; set; }

        /// <summary>
        /// Gets the associated guild.
        /// </summary>
        [ForeignKey(nameof(GuildId))]
        public DbGuild Guild { get; set; }

        /// <summary>
        /// NPC type.
        /// </summary>
        public ushort NpcType { get; set; }

        /// <summary>
        /// NPC group. Valid only for merchants, e.g. weapon, accessory etc.
        /// </summary>
        public byte Group { get; set; }

        /// <summary>
        /// NPC current level.
        /// </summary>
        public byte NpcLevel { get; set; }
    }
}
