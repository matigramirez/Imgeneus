using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("CharacterItems")]
    public sealed class DbCharacterItems : DbEntity
    {
        public byte Type { get; set; }

        public byte TypeId { get; set; }

        public byte Bag { get; set; }

        public byte Slot { get; set; }

        [Required]
        public byte Count { get; set; }

        public ushort Quality { get; set; }

        public int GemTypeId1 { get; set; }
        public int GemTypeId2 { get; set; }
        public int GemTypeId3 { get; set; }
        public int GemTypeId4 { get; set; }
        public int GemTypeId5 { get; set; }
        public int GemTypeId6 { get; set; }


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

        [ForeignKey(nameof(Type) + "," + nameof(TypeId))]
        public DbItem Item { get; set; }

        public DbCharacterItems()
        {
            CreationTime = DateTime.Now;
            Craftname = string.Empty;
        }
    }
}
