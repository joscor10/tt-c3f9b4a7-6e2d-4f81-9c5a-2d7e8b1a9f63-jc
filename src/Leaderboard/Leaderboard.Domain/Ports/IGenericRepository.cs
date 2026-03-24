using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Leaderboard.Domain.Ports
{
    public interface IGenericRepository<E> : IDisposable
       where E : Leaderboard.Domain.Entities.Base.DomainEntity
    {
        Task<IEnumerable<E>> GetAsync(Expression<Func<E, bool>>? filter, Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy, string includeStringProperties, bool isTracking);
        Task<E> GetByIdAsync(object id);
        Task<E> AddAsync(E entity);
        Task UpdateAsync(E entity);
        Task DeleteAsync(E entity);
    }
}
