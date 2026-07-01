using DAKKN.Domain.Entities;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Tests.Tests.Infrastructure
{
    public class GenericRepositoryTests : IDisposable
    {
        private readonly TestableDbContext _context;

        public GenericRepositoryTests()
        {
            _context = new TestableDbContext();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };

            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var saved = await _context.Categories.FindAsync(category.Id);
            saved.Should().NotBeNull();
            saved!.CategoryName.Should().Be("Test");
        }

        [Fact]
        public async Task AddRangeAsync_ShouldAddMultipleEntities()
        {
            var repo = new GenericRepository<Category>(_context);
            var categories = new List<Category>
            {
                new() { CategoryName = "A", ArName = "أ" },
                new() { CategoryName = "B", ArName = "ب" }
            };

            await repo.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            _context.Categories.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(category.Id);
            result.Should().NotBeNull();
            result.Id.Should().Be(category.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenEntityNotFound_ShouldThrowNotFoundException()
        {
            var repo = new GenericRepository<Category>(_context);

            var act = () => repo.GetByIdAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<DAKKN.Application.Common.Exceptions.NotFoundException>();
        }

        [Fact]
        public async Task GetAllAsync_WithoutPredicate_ShouldReturnAll()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "A", ArName = "أ" });
            _context.Categories.Add(new Category { CategoryName = "B", ArName = "ب" });
            await _context.SaveChangesAsync();

            var result = repo.GetAllAsync(null).ToList();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_WithPredicate_ShouldFilter()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "Alpha", ArName = "أ" });
            _context.Categories.Add(new Category { CategoryName = "Beta", ArName = "ب" });
            await _context.SaveChangesAsync();

            var result = repo.GetAllAsync(c => c.CategoryName == "Alpha").ToList();
            result.Should().ContainSingle();
        }

        [Fact]
        public async Task Update_ShouldModifyEntity()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Original", ArName = "أصلي" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            category.CategoryName = "Updated";
            repo.Update(category);
            await _context.SaveChangesAsync();

            var saved = await _context.Categories.FindAsync(category.Id);
            saved!.CategoryName.Should().Be("Updated");
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            repo.Delete(category);
            await _context.SaveChangesAsync();

            _context.Categories.Count().Should().Be(0);
        }

        [Fact]
        public async Task ExistsAsync_WhenExists_ShouldReturnTrue()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "Test", ArName = "اختبار" });
            await _context.SaveChangesAsync();

            var exists = await repo.ExistsAsync(c => c.CategoryName == "Test");
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByKeyAsync_WhenExists_ShouldReturnTrue()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var exists = await repo.ExistsByKeyAsync(category.Id);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task FindByKeyAsync_WhenExists_ShouldReturnEntity()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await repo.FindByKeyAsync(category.Id);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task FindByKeyAsync_WhenNotExists_ShouldReturnNull()
        {
            var repo = new GenericRepository<Category>(_context);
            var result = await repo.FindByKeyAsync(Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBy_ShouldFilter()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "Alpha", ArName = "أ" });
            _context.Categories.Add(new Category { CategoryName = "Beta", ArName = "ب" });
            await _context.SaveChangesAsync();

            var result = repo.GetBy(c => c.CategoryName.Contains("Al")).ToList();
            result.Should().ContainSingle();
        }

        [Fact]
        public async Task GetFirstAsync_ShouldReturnFirstMatch()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "Alpha", ArName = "أ" });
            await _context.SaveChangesAsync();

            var result = await repo.GetFirstAsync(c => c.CategoryName == "Alpha");
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetSingleAsync_ShouldReturnSingleMatch()
        {
            var repo = new GenericRepository<Category>(_context);
            _context.Categories.Add(new Category { CategoryName = "Alpha", ArName = "أ" });
            await _context.SaveChangesAsync();

            var result = await repo.GetSingleAsync(c => c.CategoryName == "Alpha");
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllWithIncluding_ShouldIncludeRelatedEntities()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = repo.GetAllWithIncluding(null, c => c.Products).ToList();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateRange_ShouldModifyMultipleEntities()
        {
            var repo = new GenericRepository<Category>(_context);
            var cats = new List<Category>
            {
                new() { CategoryName = "A", ArName = "أ" },
                new() { CategoryName = "B", ArName = "ب" }
            };
            await repo.AddRangeAsync(cats);
            await _context.SaveChangesAsync();

            foreach (var c in cats) c.CategoryName = "Updated";
            await repo.UpdateRange(cats);
            await _context.SaveChangesAsync();

            _context.Categories.All(c => c.CategoryName == "Updated").Should().BeTrue();
        }

        [Fact]
        public async Task GetFirstWithIncluding_ShouldReturnFirstWithIncludes()
        {
            var repo = new GenericRepository<Category>(_context);
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            await repo.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = repo.GetFirstWithIncluding(c => c.Id == category.Id, c => c.Products).ToList();
            result.Should().ContainSingle();
        }
    }

    public class TestableDbContext : DAKKN.Persistence.DAKKNDbContext
    {
        public TestableDbContext()
            : base(new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAKKN.Persistence.DAKKNDbContext>()
                .UseInMemoryDatabase($"Test_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.InvalidIncludePathError))
                .Options)
        {
            Database.EnsureCreated();
        }
    }
}
