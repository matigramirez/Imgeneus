using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imgeneus.Database.Entities
{
    [Table("Guilds")]
    public class DbGuild : DbEntity
    {
        /// <summary>
        /// Guild name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Character id, that is guild owner.
        /// </summary>
        public int MasterId { get; set; }

        /// <summary>
        /// Light or dark.
        /// </summary>
        public Fraction Country { get; set; }

        /// <summary>
        /// Guild points.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Guild's creation date.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Guild's deletion date.
        /// </summary>
        public DateTime DeleteDate { get; set; }

        /// <summary>
        /// Guild members.
        /// </summary>
        public ICollection<DbCharacter> Members { get; set; }

        public DbGuild(string name, int masterId, Fraction country)
        {
            Name = name;
            MasterId = masterId;
            Country = country;
            CreateDate = DateTime.UtcNow;
            Members = new HashSet<DbCharacter>();
        }
    }
}
