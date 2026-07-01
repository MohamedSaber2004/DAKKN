using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Products.Commands.CreateProduct;
using DAKKN.Application.Features.Products.Commands.DeleteProduct;
using DAKKN.Application.Features.Products.Commands.UpdateProduct;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetMostOrderedProducts;
using DAKKN.Application.Features.Products.Queries.GetPriceRange;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class ProductHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        public ProductHandlerTests()
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

        private async Task<Category> SeedCategory()
        {
            var cat = new Category { CategoryName = "TestCat", ArName = "تصنيف" };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return await _context.Categories.FirstAsync();
        }

        private static CreateProductCommand MakeCreateCmd(Guid catId) => new(
            "Test Product", "منتج اختبار", "Description", "وصف",
            100m, "img.jpg", new List<string>(), new List<string>(),
            catId, 10, 3);

        [Fact]
        public async Task CreateProduct_ShouldReturnDto()
        {
            var cat = await SeedCategory();
            var handler = new CreateProductCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(MakeCreateCmd(cat.Id), default);

            result.Name.Should().Be("Test Product");
            result.Price.Should().Be(100m);
        }

        [Fact]
        public async Task CreateProduct_InvalidCategory_ShouldThrow()
        {
            var handler = new CreateProductCommandHandler(_unitOfWork, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(MakeCreateCmd(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteProduct_ShouldSoftDelete()
        {
            var cat = await SeedCategory();
            var prod = new Product { Name = "P", ArName = "م", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1 };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new DeleteProductCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new DeleteProductCommand(prod.Id), default);

            var deleted = await _context.Products.IgnoreQueryFilters().FirstAsync(p => p.Id == prod.Id);
            deleted.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task GetProductById_ShouldReturnDto()
        {
            var cat = await SeedCategory();
            var prod = new Product { Name = "P", ArName = "م", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1 };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetProductByIdQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductByIdQuery(prod.Id), default);

            result.Should().NotBeNull();
            result.Name.Should().Be("P");
        }

        [Fact]
        public async Task GetProductById_NotFound_ShouldThrow()
        {
            var handler = new GetProductByIdQueryHandler(_unitOfWork);
            await FluentActions.Invoking(() => handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetPriceRange_ShouldReturnMinMax()
        {
            var cat = await SeedCategory();
            _context.Products.AddRange(
                new Product { Name = "A", ArName = "أ", Description = "D", ArDescription = "و", Price = 50m, CategoryId = cat.Id, QuantityInStock = 1 },
                new Product { Name = "B", ArName = "ب", Description = "D", ArDescription = "و", Price = 200m, CategoryId = cat.Id, QuantityInStock = 1 });
            await _context.SaveChangesAsync();

            var handler = new GetPriceRangeQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetPriceRangeQuery(), default);

            result.MinPrice.Should().Be(50m);
            result.MaxPrice.Should().Be(200m);
        }

        [Fact]
        public async Task GetFeaturedProducts_ShouldReturnNewArrivals()
        {
            var cat = await SeedCategory();
            _context.Products.Add(new Product { Name = "A", ArName = "أ", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1 });
            await _context.SaveChangesAsync();

            var handler = new GetFeaturedProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetFeaturedProductsQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProducts_ShouldPaginate()
        {
            var cat = await SeedCategory();
            for (int i = 0; i < 5; i++)
                _context.Products.Add(new Product { Name = $"P{i}", ArName = $"م{i}", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1 });
            await _context.SaveChangesAsync();

            var handler = new GetProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductsQuery(null, null, 1, 2), default);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetProducts_FilterByCategory()
        {
            var cat1 = await SeedCategory();
            var cat2 = new Category { CategoryName = "Other", ArName = "آخر" };
            _context.Categories.Add(cat2);
            await _context.SaveChangesAsync();
            _context.Products.AddRange(
                new Product { Name = "A", ArName = "أ", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat1.Id, QuantityInStock = 1 },
                new Product { Name = "B", ArName = "ب", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat2.Id, QuantityInStock = 1 });
            await _context.SaveChangesAsync();

            var handler = new GetProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductsQuery(null, cat1.Id, 1, 20), default);

            result.Items.Should().HaveCount(1);
            result.Items.First().Name.Should().Be("A");
        }
    }
}
