using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Categories.Commands.CreateCategory;
using DAKKN.Application.Features.Categories.Commands.DeleteCategory;
using DAKKN.Application.Features.Categories.Commands.UpdateCategory;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class CategoryHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        public CategoryHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnDto()
        {
            var handler = new CreateCategoryCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(new CreateCategoryCommand("NewCat", "تصنيف جديد", null), default);

            result.CategoryName.Should().Be("NewCat");
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_ShouldThrow()
        {
            _context.Categories.Add(new Category { CategoryName = "Existing", ArName = "موجود" });
            await _context.SaveChangesAsync();
            var handler = new CreateCategoryCommandHandler(_unitOfWork, _localizerMock.Object);

            await FluentActions.Invoking(() => handler.Handle(new CreateCategoryCommand("Existing", "جديد", null), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task DeleteCategory_ShouldSoftDelete()
        {
            var cat = new Category { CategoryName = "Test", ArName = "اختبار" };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new DeleteCategoryCommandHandler(_unitOfWork, _currentUserMock.Object);
            await handler.Handle(new DeleteCategoryCommand(cat.Id), default);

            var deleted = await _context.Categories.IgnoreQueryFilters().FirstAsync(c => c.Id == cat.Id);
            deleted.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteCategory_NotFound_ShouldThrow()
        {
            var handler = new DeleteCategoryCommandHandler(_unitOfWork, _currentUserMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteCategoryCommand(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateCategory_ShouldUpdateName()
        {
            var cat = new Category { CategoryName = "Old", ArName = "قديم" };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new UpdateCategoryCommandHandler(_unitOfWork, _localizerMock.Object, _currentUserMock.Object);
            var result = await handler.Handle(new UpdateCategoryCommand(cat.Id, "NewName", "اسم جديد", null, true), default);

            result.CategoryName.Should().Be("NewName");
        }

        [Fact]
        public async Task UpdateCategory_NotFound_ShouldThrow()
        {
            var handler = new UpdateCategoryCommandHandler(_unitOfWork, _localizerMock.Object, _currentUserMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new UpdateCategoryCommand(Guid.NewGuid(), "N", "ج", null, true), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetCategories_ShouldReturnActiveOnly()
        {
            _context.Categories.AddRange(
                new Category { CategoryName = "A", ArName = "أ", IsDeleted = false },
                new Category { CategoryName = "B", ArName = "ب", IsDeleted = true });
            await _context.SaveChangesAsync();
            var handler = new GetCategoriesQueryHandler(_unitOfWork);

            var result = await handler.Handle(new GetCategoriesQuery(null, false, null), default);

            result.Should().HaveCount(1);
            result[0].CategoryName.Should().Be("A");
        }

        [Fact]
        public async Task GetCategories_IncludeInactive_ShouldReturnAll()
        {
            _context.Categories.AddRange(
                new Category { CategoryName = "A", ArName = "أ", IsDeleted = false },
                new Category { CategoryName = "B", ArName = "ب", IsDeleted = true });
            await _context.SaveChangesAsync();
            var handler = new GetCategoriesQueryHandler(_unitOfWork);

            var result = await handler.Handle(new GetCategoriesQuery(null, true, null), default);

            result.Should().HaveCount(2);
        }
    }
}
