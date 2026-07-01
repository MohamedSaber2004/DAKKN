using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Common;
using DAKKN.Domain.Entities;
using DAKKN.Persistence;
using DAKKN.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DAKKN.Tests.Tests.Persistence
{
    public class SaveChangesInterceptorTests
    {
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly AuditInterceptor _interceptor;

        public SaveChangesInterceptorTests()
        {
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(x => x.UserId).Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            _currentUserMock.Setup(x => x.UserName).Returns("TestUser");
            _interceptor = new AuditInterceptor(_currentUserMock.Object);
        }

        [Fact]
        public async Task AddedEntity_ShouldSetCreatedAuditFields()
        {
            using var context = CreateContext();
            var category = new Category { CategoryName = "New", ArName = "جديد" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            category.CreatedBy.Should().Be("11111111-1111-1111-1111-111111111111");
            category.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task ModifiedEntity_ShouldSetUpdatedAuditFields()
        {
            using var context = CreateContext();
            var category = new Category { CategoryName = "Original", ArName = "أصلي" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            category.CategoryName = "Modified";
            await context.SaveChangesAsync();

            category.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            category.UpdatedBy.Should().Be("11111111-1111-1111-1111-111111111111");
        }

        [Fact]
        public async Task DeletedEntity_ShouldSoftDelete()
        {
            using var context = CreateContext();
            var category = new Category { CategoryName = "Test", ArName = "اختبار" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            category.IsDeleted.Should().BeTrue();
            category.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            category.DeletedBy.Should().Be("11111111-1111-1111-1111-111111111111");

            var stillInDb = await context.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == category.Id);
            stillInDb.Should().NotBeNull();
        }

        [Fact]
        public async Task MultipleEntities_ShouldEachGetAuditFields()
        {
            using var context = CreateContext();
            var cat1 = new Category { CategoryName = "A", ArName = "أ" };
            var cat2 = new Category { CategoryName = "B", ArName = "ب" };
            context.Categories.AddRange(cat1, cat2);
            await context.SaveChangesAsync();

            cat1.CreatedBy.Should().Be("11111111-1111-1111-1111-111111111111");
            cat2.CreatedBy.Should().Be("11111111-1111-1111-1111-111111111111");
        }

        [Fact]
        public void UpdateEntities_WithNullContext_ShouldNotThrow()
        {
            var act = () => _interceptor.UpdateEntities(null);
            act.Should().NotThrow();
        }

        private DAKKNDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DAKKNDbContext>()
                .UseInMemoryDatabase($"Audit_Test_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(CoreEventId.InvalidIncludePathError))
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .AddInterceptors(_interceptor)
                .Options;

            var context = new DAKKNDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
