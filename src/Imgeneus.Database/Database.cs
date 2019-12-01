using Imgeneus.Database.Context;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Imgeneus.Database
{
    public sealed class Database : IDatabase
    {
        /// <inheritdoc />
        public DbSet<DbUser> Users { get => DatabaseContext.Users; }

        /// <inheritdoc />
        public DbSet<DbCharacter> Charaters { get => DatabaseContext.Characters; }

        /// <inheritdoc />
        public DbSet<DbCharacterItems> CharacterItems { get => DatabaseContext.CharacterItems; }

        public DatabaseContext DatabaseContext { get; private set; }

        public Database(DatabaseContext databaseContext)
        {
            this.DatabaseContext = databaseContext;
        }

        /// <inheritdoc />
        public void Complete() => this.DatabaseContext.SaveChanges();

        /// <inheritdoc />
        public async Task CompleteAsync() => await this.DatabaseContext.SaveChangesAsync();

        /// <inheritdoc />
        public void Dispose() => this.DatabaseContext.Dispose();

    }
}
