using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imgeneus.Database
{
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets the users.
        /// </summary>
        public DbSet<DbUser> Users { get; set; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public DbSet<DbCharacter> Characters { get; set; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public DbSet<DbCharacterItems> CharacterItems { get; set; }

        /// <summary>
        /// Gets or sets chracter skills.
        /// </summary>
        public DbSet<DbCharacterSkill> CharacterSkills { get; set; }

        /// <summary>
        /// Gets the skills.
        /// </summary>
        public DbSet<DbSkill> Skills { get; set; }

        /// <summary>
        /// Collection of items. Taken from original db.
        /// </summary>
        public DbSet<DbItem> Items { get; set; }

        /// <summary>
        /// Collection of characters' active buffs.
        /// </summary>
        public DbSet<DbCharacterActiveBuff> ActiveBuffs { get; set; }

        /// <summary>
        /// Saves changes to database.
        /// </summary>
        public int SaveChanges();

        /// <summary>
        /// Saves changes to database.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
