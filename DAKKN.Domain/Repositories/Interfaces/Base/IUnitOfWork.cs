using DAKKN.Domain.Common.Interfaces;

namespace DAKKN.Domain.Repositories.Interfaces.Base
{
    public interface IUnitOfWork : IDisposable
    {
        IAttachmentRepository AttachmentRepository { get; }

        IGenericRepository<T, TKey> GetRepository<T, TKey>() where T : class, IBaseEntity<TKey> where TKey : IEquatable<TKey>;
        IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity<Guid>;

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
