using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Imgeneus.Database.Context
{
    public class DatabaseContext : DbContext, IDatabase
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
        /// Gets or sets chracter skills.
        /// </summary>
        public DbSet<DbCharacterSkill> CharacterSkills { get; set; }

        /// <summary>
        /// Collection of skills. Taken from original db.
        /// </summary>
        public DbSet<DbSkill> Skills { get; set; }

        /// <summary>
        /// Collection of items. Taken from original db.
        /// </summary>
        public DbSet<DbItem> Items { get; set; }

        /// <summary>
        /// Collection of mobs. Taken from original db.
        /// </summary>
        public DbSet<DbMob> Mobs { get; set; }

        /// <summary>
        /// Available drop from a monster. Taken from original db.
        /// </summary>
        public DbSet<DbMobItems> MobItems { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>().HasIndex(c => new { c.Username, c.Email }).IsUnique();

            modelBuilder.Entity<DbSkill>().HasIndex(s => new { s.SkillId, s.SkillLevel });

            modelBuilder.Entity<DbItem>().HasKey(x => new { x.Type, x.TypeId });

            modelBuilder.Entity<DbMobItems>().HasKey(x => new { x.MobId, x.ItemOrder });

            #region Many to many relations
            // Skills.
            modelBuilder.Entity<DbCharacterSkill>().HasKey(x => new { x.CharacterId, x.SkillId });
            modelBuilder.Entity<DbCharacterSkill>().HasOne(pt => pt.Character).WithMany(p => p.Skills).HasForeignKey(pt => pt.CharacterId);

            // Items
            modelBuilder.Entity<DbCharacterItems>().HasOne(pt => pt.Character).WithMany(p => p.Items).HasForeignKey(pt => pt.CharacterId);
            #endregion
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
