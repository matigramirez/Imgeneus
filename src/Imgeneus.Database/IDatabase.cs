using Imgeneus.Database.Context;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Imgeneus.Database
{
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets or sets the database context.
        /// </summary>
        DatabaseContext DatabaseContext { get; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        public DbSet<DbUser> Users { get; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public DbSet<DbCharacter> Charaters { get; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public DbSet<DbCharacterItems> CharacterItems { get; }

        /// <summary>
        /// Complete the pending database operation.
        /// </summary>
        void Complete();

        /// <summary>
        /// Complete the pending database operations in an asynchronous context.
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}
