using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Tests.Tests.Infrastructure
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly TestableDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UnitOfWork _uow;

        public UnitOfWorkTests()
        {
            _context = new TestableDbContext();
            var services = new ServiceCollection();
            _serviceProvider = services.BuildServiceProvider();
            _uow = new UnitOfWork(_context, _serviceProvider);
        }

        public void Dispose()
        {
            _uow.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void GetRepository_WithoutTypeParams_ShouldReturnGenericRepository()
        {
            var repo = _uow.GetRepository<Category>();
            repo.Should().NotBeNull();
            repo.Should().BeAssignableTo<IGenericRepository<Category>>();
        }

        [Fact]
        public void GetRepository_WithTypeParams_ShouldReturnGenericRepository()
        {
            var repo = _uow.GetRepository<Category, Guid>();
            repo.Should().NotBeNull();
            repo.Should().BeAssignableTo<IGenericRepository<Category, Guid>>();
        }

        [Fact]
        public void GetRepository_CalledTwice_ShouldReturnSameInstance()
        {
            var repo1 = _uow.GetRepository<Category>();
            var repo2 = _uow.GetRepository<Category>();
            repo1.Should().BeSameAs(repo2);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChanges()
        {
            var repo = _uow.GetRepository<Category>();
            await repo.AddAsync(new Category { CategoryName = "Test", ArName = "اختبار" });

            var count = await _uow.SaveChangesAsync();
            count.Should().Be(1);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
        {
            var count = await _uow.SaveChangesAsync();
            count.Should().Be(0);
        }

        [Fact(Skip = "InMemory database does not support transactions")]
        public async Task BeginTransactionAsync_ShouldStartTransaction()
        {
            await _uow.BeginTransactionAsync();
            var repo = _uow.GetRepository<Category>();
            await repo.AddAsync(new Category { CategoryName = "Test", ArName = "اختبار" });
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            _context.Categories.Count().Should().Be(1);
        }

        [Fact(Skip = "InMemory database does not support transactions")]
        public async Task RollbackAsync_ShouldDisposeTransaction()
        {
            await _uow.BeginTransactionAsync();
            var repo = _uow.GetRepository<Category>();
            await repo.AddAsync(new Category { CategoryName = "Test", ArName = "اختبار" });
            await _uow.SaveChangesAsync();
            await _uow.RollbackAsync();

            _context.ChangeTracker.Clear();
            _context.Categories.Count().Should().Be(0);
        }

        [Fact]
        public async Task CommitAsync_WithoutTransaction_ShouldNotThrow()
        {
            var act = () => _uow.CommitAsync();
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task RollbackAsync_WithoutTransaction_ShouldNotThrow()
        {
            var act = () => _uow.RollbackAsync();
            await act.Should().NotThrowAsync();
        }

        [Fact(Skip = "InMemory database does not support transactions")]
        public async Task BeginTransaction_Commit_Rollback_ShouldWorkInSequence()
        {
            await _uow.BeginTransactionAsync();
            var repo = _uow.GetRepository<Category>();
            await repo.AddAsync(new Category { CategoryName = "A", ArName = "أ" });
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            await _uow.BeginTransactionAsync();
            await repo.AddAsync(new Category { CategoryName = "B", ArName = "ب" });
            await _uow.SaveChangesAsync();
            await _uow.RollbackAsync();

            _context.ChangeTracker.Clear();
            _context.Categories.Count().Should().Be(1);
            _context.Categories.Single().CategoryName.Should().Be("A");
        }

        [Fact]
        public void Dispose_ShouldReleaseContext()
        {
            var act = () => _uow.Dispose();
            act.Should().NotThrow();
        }

        [Fact]
        public void GetRepository_WhenRegisteredInDi_ShouldReturnRegisteredInstance()
        {
            var services = new ServiceCollection();
            var customRepo = new Mock<IGenericRepository<Category>>();
            services.AddSingleton(customRepo.Object);
            var sp = services.BuildServiceProvider();

            var uow = new UnitOfWork(_context, sp);
            var repo = uow.GetRepository<Category>();
            repo.Should().BeSameAs(customRepo.Object);
        }
    }
}
