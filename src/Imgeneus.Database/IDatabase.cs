using Imgeneus.Database.Context;
using Imgeneus.Database.Entities;
using Imgeneus.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace Imgeneus.Database
{
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets or sets the database context.
        /// </summary>
        DatabaseContext DatabaseContext { get; set; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        public IRepository<DbUser> Users { get; set; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public IRepository<DbCharacter> Charaters { get; set; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        public IRepository<DbCharacterItems> CharacterItems { get; set; }

        /// <summary>
        /// Gets the skills.
        /// </summary>
        public IRepository<DbSkill> Skills { get; set; }

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
