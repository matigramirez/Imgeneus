using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterQuickItems")]
    public class DbQuickSkillBarItem : DbEntity
    {
        /// <summary>
        /// Character to whom this quick item belongs to.
        /// </summary>
        public DbCharacter Character { get; set; }

        [ForeignKey(nameof(Character))]
        public int CharacterId { get; set; }

        /// <summary>
        /// Quick bar index.
        /// </summary>
        public byte Bar { get; set; }

        /// <summary>
        /// Quick slot index.
        /// </summary>
        public byte Slot { get; set; }

        /// <summary>
        /// Bag type. For "usual" inventory items it can be 0-5.
        /// For skills it's always 100.
        /// For motion 100+.
        /// </summary>
        public byte Bag { get; set; }

        /// <summary>
        /// For "usual" inventory items it's slot id.
        /// For skills it's skill id.
        /// </summary>
        public ushort Number { get; set; }
    }
}
