using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterItems")]
    public sealed class DbCharacterItems : DbEntity
    {
        [Required]
        public byte Type { get; set; }

        [Required]
        public byte TypeId { get; set; }

        public byte Bag { get; set; }

        public byte Slot { get; set; }

        [Required]
        public byte Count { get; set; }

        public ushort Quality { get; set; }

        [Required]
        [MaxLength(6)]
        public byte[] Gems { get; set; }

        [Required]
        [MaxLength(20)]
        public string Craftname { get; set; }

        public DateTime CreationTime { get; set; }

        public char MakeType { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the item is deleted.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the character Id associated to this item.
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the character associated to this item.
        /// </summary>
        public DbCharacter Character { get; set; }

        public DbCharacterItems()
        {
            Gems = new byte[6];
            CreationTime = DateTime.Now;
            Craftname = string.Empty;
        }
    }
}
