using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Levels")]
    public class DbLevel : DbEntity
    {
        /// <summary>
        /// Required level
        /// </summary>
        [Required]
        public ushort Level { get; set; }

        /// <summary>
        /// Required mode
        /// </summary>
        [Required]
        public Mode Mode { get; set; }

        /// <summary>
        /// Required experience for next level
        /// </summary>
        [Required]
        public uint Exp { get; set; }
    }
}