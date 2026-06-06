using DAKKN.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace DAKKN.Domain.Repositories.Interfaces.Base
{
    public interface IGenericRepository<T, TKey>
       where T : class, IBaseEntity<TKey>
       where TKey : IEquatable<TKey>
    {
        IQueryable<T> GetAllAsync(Expression<Func<T, bool>>? predicate);
        Task<T> GetByIdAsync(TKey id);
        IQueryable<T> GetBy(Expression<Func<T, bool>> predicate);
        Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        Task UpdateRange(IEnumerable<T> entities);

        void Delete(T entity);


        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> ExistsByKeyAsync(TKey key, CancellationToken cancellationToken = default);
        Task<T?> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default);

        IQueryable<T> GetAllWithIncluding(Expression<Func<T, bool>>? predicate, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetFirstWithIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    }

    public interface IGenericRepository<T> : IGenericRepository<T, Guid>
        where T : class, IBaseEntity<Guid>
    {
    }
}
