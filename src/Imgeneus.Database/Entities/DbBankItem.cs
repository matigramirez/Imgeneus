using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("BankItems")]
    public class DbBankItem : DbEntity
    {
        /// <summary>
        /// Gets the bank item associated user id.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        [Required]
        public byte Type { get; set; }

        [Required]
        public byte TypeId { get; set; }

        [Required]
        [Range(0, 239)]
        public byte Slot { get; set; }

        [Required]
        public byte Count { get; set; }

        /// <summary>
        /// Time at which the player obtained the item.
        /// </summary>
        [Required]
        public DateTime ObtainmentTime { get; set; }

        /// <summary>
        /// Time at which the player got the item from the bank in-game.
        /// </summary>
        public DateTime? ClaimTime { get; set; }

        [DefaultValue(false)]
        public bool IsClaimed { get; set; }

        /// <summary>
        /// Flag that indicates if the bank item is deleted.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// The bank item's associated item.
        /// </summary>
        [ForeignKey(nameof(Type) + "," + nameof(TypeId))]
        public DbItem Item { get; set; }

        /// <summary>
        /// The bank item's associated user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public DbUser User { get; set; }
    }
}
