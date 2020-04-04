using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("MobItems")]
    public class DbMobItems
    {
        /// <summary>
        /// Unique mob id.
        /// </summary>
        public ushort MobId { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public byte ItemOrder { get; set; }

        /// <summary>
        /// Item grade.
        /// </summary>
        public ushort Grade { get; set; }

        /// <summary>
        /// Percentage/possibility of the drop.
        /// </summary>
        public int DropRate { get; set; }
    }
}
