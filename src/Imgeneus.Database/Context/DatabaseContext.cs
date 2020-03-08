using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Imgeneus.Database.Context
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Gets or sets users.
        /// </summary>
        public DbSet<DbUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the characters.
        /// </summary>
        public DbSet<DbCharacter> Characters { get; set; }

        /// <summary>
        /// Gets or sets the character items.
        /// </summary>
        public DbSet<DbCharacterItems> CharacterItems { get; set; }

        /// <summary>
        /// Collection of skills. Taken from original db and change colums.
        /// </summary>
        public DbSet<DbSkill> Skills { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>().HasIndex(c => new { c.Username, c.Email }).IsUnique();

            modelBuilder.Entity<DbSkill>().HasIndex(s => new { s.SkillId, s.SkillLevel });

            // Many to many relations.
            modelBuilder.Entity<DbCharacterSkill>().HasKey(x => new { x.CharacterId, x.SkillId });
            modelBuilder.Entity<DbCharacterSkill>().HasOne(pt => pt.Character).WithMany(p => p.Skills).HasForeignKey(pt => pt.CharacterId);
        }

        /// <summary>
        /// Migrates the database schema.
        /// </summary>
        public void Migrate() => this.Database.Migrate();

        /// <summary>
        /// Check if the database exists.
        /// </summary>
        /// <returns></returns>
        public bool DatabaseExists() => (this.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
    }
}
