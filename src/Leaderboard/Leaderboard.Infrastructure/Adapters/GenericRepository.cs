using Leaderboard.Domain.Ports;
using Leaderboard.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Leaderboard.Infrastructure.Adapters
{
    public class GenericRepository<E> : IGenericRepository<E> where E : Leaderboard.Domain.Entities.Base.DomainEntity
    {
        readonly PersistenceContext _context;
        public GenericRepository(PersistenceContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<E> AddAsync(E entity)
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity), "Entity can not be null");
            _context.Set<E>().Add(entity);
            await this.CommitAsync();
            return entity;
        }

        public async Task<E> GetByIdAsync(object id)
        {
            return await _context.Set<E>().FindAsync(id);
        }
        public async Task UpdateAsync(E entity)
        {
            if (entity != null)
            {
                _context.Set<E>().Update(entity);
                await this.CommitAsync();
            }
        }
        public async Task DeleteAsync(E entity)
        {
            if (entity != null)
            {
                _context.Set<E>().Remove(entity);
                await this.CommitAsync();
            }
        }


        public async Task<IEnumerable<E>> GetAsync(
            Expression<Func<E, bool>>? filter,
            Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy,
            string includeStringProperties,
            bool isTracking)
        {
            IQueryable<E> query = _context.Set<E>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeStringProperties))
            {
                foreach (var includeProperty in includeStringProperties.Split
                    (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            }

            return (!isTracking) ? await query.AsNoTracking().ToListAsync() : await query.ToListAsync();
        }

        public async Task CommitAsync()
        {
            _context.ChangeTracker.DetectChanges();
            await _context.CommitAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this._context.Dispose();
        }
    }
}
