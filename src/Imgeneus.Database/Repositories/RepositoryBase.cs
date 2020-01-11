using Imgeneus.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Imgeneus.Database.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : DbEntity
    {
        private readonly DbContext context;

        /// <summary>
        /// Initializes the repository <see cref="DbContext"/>.
        /// </summary>
        /// <param name="context"></param>
        public RepositoryBase(DbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public T Create(T entity)
        {
            this.context.Set<T>().Add(entity);

            return entity;
        }

        /// <inheritdoc />
        public async Task<T> CreateAsync(T entity)
        {
            var trackedEntity = await this.context.Set<T>().AddAsync(entity);

            return trackedEntity.Entity;
        }

        /// <inheritdoc />
        public T Update(T entity)
        {
            var trackedEntity = this.context.Set<T>().Update(entity);

            return trackedEntity.Entity;
        }

        /// <inheritdoc />
        public T Delete(T entity)
        {
            var trackedEntity = this.context.Set<T>().Remove(entity);

            return trackedEntity.Entity;
        }

        /// <inheritdoc />
        public T Get(int id) => this.Get(x => x.Id == id);

        /// <inheritdoc />
        public T Get(Func<T, bool> func) => this.GetQueryable(this.context).FirstOrDefault(func);

        /// <inheritdoc />
        public IEnumerable<T> GetAll() => this.GetQueryable(this.context).AsEnumerable();

        /// <inheritdoc />
        public IEnumerable<T> GetAll(Func<T, bool> func) => this.GetQueryable(this.context).Where(func).AsEnumerable();

        /// <inheritdoc />
        public IQueryable<T> Include(Expression<Func<T, object>> criteria) => context.Set<T>().Include(criteria);

        /// <inheritdoc />
        public int Count() => this.context.Set<T>().AsNoTracking().Count();

        /// <inheritdoc />
        public int Count(Func<T, bool> func) => this.context.Set<T>().AsNoTracking().Count(func);

        /// <inheritdoc />
        public bool HasAny(Func<T, bool> predicate) => this.context.Set<T>().AsNoTracking().Any(predicate);

        /// <inheritdoc />
        protected virtual IQueryable<T> GetQueryable(DbContext context) => context.Set<T>().AsQueryable();
    }
}
