using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Users_Master")]
    public class DbUser
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserUID")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        [Required]
        [MaxLength(19)]
        [Column("UserID")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        [MaxLength(16)]
        [Column("Pw")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's current status.
        /// </summary>
        [DefaultValue(0)]
        [Column("Status")]
        public UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the user's current status.
        /// </summary>
        [DefaultValue(0)]
        [Column("AdminLevel")]
        public byte Authority { get; set; }

        /// <summary>
        /// Gets or sets the users's current points.
        /// </summary>
        [Column("Point")]
        public int Points { get; set; }

        /// <summary>
        /// Gets the user's creation time.
        /// </summary>
        [Column("JoinDate"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Gets or sets the last time user login.
        /// </summary>
        [Column("LeaveDate")]
        public DateTime LastConnectionTime { get; set; }

    }

    public enum UserStatus : short
    {
        Active,
        Banned, // This will show a message, that the account doesn't exist. Maybe can be used for banned accounts.
        NotFree, // No idea how this can be used... It will show "account is not selected as free challenger".
        WrongUsernameOrPassword
    }
}
