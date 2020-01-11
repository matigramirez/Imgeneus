using Imgeneus.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace Imgeneus.Database.Repositories
{
    /// <summary>
    /// Describes the repository behavior.
    /// </summary>
    /// <typeparam name="T">The database entity.</typeparam>
    public interface IRepository<T> where T : DbEntity
    {
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The database entity.</param>
        /// <returns>The database entity.</returns>
        T Create(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The database entity.</param>
        /// <returns>The database entity.</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The database entity.</param>
        /// <returns>The database entity.</returns>
        T Update(T entity);

        /// <summary>
        /// Delete an existing entity.
        /// </summary>
        /// <param name="entity">The database entity.</param>
        /// <returns>The database entity.</returns>
        T Delete(T entity);

        /// <summary>
        /// Gets an entity by his Id.
        /// </summary>
        /// <param name="id">The database entity.</param>
        /// <returns>The database entity.</returns>
        T Get(int id);

        /// <summary>
        /// Gets an entity with a filter expresion.
        /// </summary>
        /// <param name="func"></param>
        /// <returns>The database entity.</returns>
        T Get(Func<T, bool> func);

        /// <summary>
        /// Get all records from the repository.
        /// </summary>
        /// <returns>All the database entities.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets all records from the repository with a filter expression.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        IEnumerable<T> GetAll(Func<T, bool> func);

        /// <summary>
        /// Performs tables left join.
        /// </summary>
        IQueryable<T> Include(Expression<Func<T, object>> criteria);

        /// <summary>
        /// Get the total amount of records from the repository
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Get the total amount of records from the repository with a filter expression.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        int Count(Func<T, bool> func);

        /// <summary>
        /// Check if there is entities that matches the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool HasAny(Func<T, bool> predicate);
    }
}
