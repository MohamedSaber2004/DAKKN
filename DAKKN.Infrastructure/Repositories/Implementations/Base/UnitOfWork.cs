using DAKKN.Domain.Common.Interfaces;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Domain.Repositories.Interfaces.Base;
using DAKKN.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Infrastructure.Repositories.Implementations.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DAKKNDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, object> _repositories;
        private IDbContextTransaction? _transaction;

        private IAttachmentRepository? _attachmentRepository;

        public UnitOfWork(DAKKNDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _repositories = new Dictionary<string, object>();
        }

        public IAttachmentRepository AttachmentRepository => _attachmentRepository ?? new AttachmentRepository(_context);

        public IGenericRepository<T, TKey> GetRepository<T, TKey>() where T : class, IBaseEntity<TKey> where TKey : IEquatable<TKey>
        {
            var key = $"{typeof(T).Name}_{typeof(TKey).Name}";

            if (!_repositories.ContainsKey(key))
            {
                var repositoryInstance = _serviceProvider.GetService<IGenericRepository<T, TKey>>() 
                                         ?? new GenericRepository<T, TKey>(_context);

                _repositories.Add(key, repositoryInstance);
            }

            return (IGenericRepository<T, TKey>)_repositories[key];
        }

        public IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity<Guid>
        {
            var key = $"{typeof(T).Name}_Guid";

            if (!_repositories.ContainsKey(key))
            {
                var repositoryInstance = _serviceProvider.GetService<IGenericRepository<T>>() 
                                         ?? new GenericRepository<T>(_context);

                _repositories.Add(key, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[key];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
