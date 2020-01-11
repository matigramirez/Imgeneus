using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Users")]
    public class DbUser : DbEntity
    {
        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        [Required]
        [MaxLength(19)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        [MaxLength(16)]
        public string Password { get; set; }

        /// <summary>
        /// Gets pr sets the user's email address.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's current status.
        /// </summary>
        [DefaultValue(0)]
        public byte Status { get; set; }

        /// <summary>
        /// Gets or sets the user's current status.
        /// </summary>
        [DefaultValue(0)]
        public byte Authority { get; set; }

        /// <summary>
        /// Gets or sets the users's current points.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the user current faction.
        /// </summary>
        [DefaultValue(Fraction.NotSelected)]
        public Fraction Faction { get; set; }

        /// <summary>
        /// Gets or sets the user current maximum mode allowed.
        /// </summary>
        [DefaultValue(0)]
        public byte MaxMode { get; set; }

        /// <summary>
        /// Gets the user's creation time.
        /// </summary>
        [Column(TypeName = "DATETIME")]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// Gets or sets the last time user login.
        /// </summary>
        [Column(TypeName = "DATETIME")]
        public DateTime LastConnectionTime { get; set; }

        /// <summary>
        /// Gets or sets the user's characters list.
        /// </summary>
        public ICollection<DbCharacter> Characters { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the user is deleted.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Creates a new <see cref="DbUser"/> instance.
        /// </summary>
        public DbUser()
        {
            this.CreateTime = DateTime.Now;
            this.Characters = new HashSet<DbCharacter>();
        }

    }

    public enum Fraction : byte
    {
        Light = 0,
        Dark = 1,
        NotSelected = 2
    }
}
