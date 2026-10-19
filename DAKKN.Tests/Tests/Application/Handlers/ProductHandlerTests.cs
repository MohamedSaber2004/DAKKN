using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Products.Commands.CreateProduct;
using DAKKN.Application.Features.Products.Commands.DeleteProduct;
using DAKKN.Application.Features.Products.Commands.ToggleProductFeatured;
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
            100m, "img.jpg", new List<string>(), new List<string>(), new List<string>(),
            catId, 10, 3);

        private static CreateProductCommand MakeCreateCmd(Guid catId, List<string> images) => new(
            "Test Product", "منتج اختبار", "Description", "وصف",
            100m, images.FirstOrDefault() ?? string.Empty, images, new List<string>(), new List<string>(),
            catId, 10, 3);

        private async Task<Product> SeedProductWithImages(Guid catId, List<string> images)
        {
            var prod = new Product
            {
                Name = "P",
                ArName = "م",
                Description = "D",
                ArDescription = "و",
                Price = 10m,
                CategoryId = catId,
                QuantityInStock = 1,
                ImageUrl = images.FirstOrDefault() ?? string.Empty,
                Images = images
            };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return prod;
        }

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
        public async Task GetFeaturedProducts_WhenNoFeaturedProducts_ShouldReturnEmptyList()
        {
            var cat = await SeedCategory();
            _context.Products.Add(new Product { Name = "A", ArName = "أ", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1, IsFeatured = false });
            await _context.SaveChangesAsync();

            var handler = new GetFeaturedProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetFeaturedProductsQuery(), default);

            result.Should().BeEmpty();
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

        [Fact]
        public async Task CreateProduct_WithImages_ShouldStoreListAndDeriveCover()
        {
            var cat = await SeedCategory();
            var images = new List<string> { "a.jpg", "b.jpg", "c.jpg" };
            var handler = new CreateProductCommandHandler(_unitOfWork, _localizerMock.Object);

            var result = await handler.Handle(MakeCreateCmd(cat.Id, images), default);

            result.Images.Should().Equal(images);
            // Cover invariant: ImageUrl mirrors the first gallery image.
            result.ImageUrl.Should().Be("a.jpg");
        }

        [Fact]
        public async Task CreateProduct_WithNoImages_ShouldFallBackToImageUrlParam()
        {
            var cat = await SeedCategory();
            var handler = new CreateProductCommandHandler(_unitOfWork, _localizerMock.Object);

            // The single-argument overload is the legacy path: a cover supplied directly with no
            // gallery. The two-argument overload derives ImageUrl from the list and would send "".
            var result = await handler.Handle(MakeCreateCmd(cat.Id), default);

            result.Images.Should().BeEmpty();
            result.ImageUrl.Should().Be("img.jpg");
        }

        [Fact]
        public async Task UpdateProduct_WhenCoverRemoved_ShouldPromoteNextImage()
        {
            var cat = await SeedCategory();
            var prod = await SeedProductWithImages(cat.Id, new List<string> { "a.jpg", "b.jpg", "c.jpg" });
            var handler = new UpdateProductCommandHandler(_unitOfWork, _localizerMock.Object);

            // Admin deletes the cover (a.jpg) from the form.
            var remaining = new List<string> { "b.jpg", "c.jpg" };
            var result = await handler.Handle(new UpdateProductCommand(
                prod.Id, "P", "م", "D", "و", 10m,
                prod.ImageUrl, remaining, new List<string>(), new List<string>(),
                cat.Id, 1, 0), default);

            result.Images.Should().Equal(remaining);
            result.ImageUrl.Should().Be("b.jpg");
        }

        [Fact]
        public async Task UpdateProduct_RemovingAllImages_ShouldClearCover()
        {
            var cat = await SeedCategory();
            var prod = await SeedProductWithImages(cat.Id, new List<string> { "a.jpg" });
            var handler = new UpdateProductCommandHandler(_unitOfWork, _localizerMock.Object);

            // ImageUrl arrives as string.Empty when the admin removed every image — this is what
            // AdminController passes (finalImages.FirstOrDefault() ?? string.Empty). With no
            // gallery left there is no cover to fall back to, so the cover clears.
            var result = await handler.Handle(new UpdateProductCommand(
                prod.Id, "P", "م", "D", "و", 10m,
                string.Empty, new List<string>(), new List<string>(), new List<string>(),
                cat.Id, 1, 0), default);

            result.Images.Should().BeEmpty();
            result.ImageUrl.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateProduct_Reordering_ShouldMoveCover()
        {
            var cat = await SeedCategory();
            var prod = await SeedProductWithImages(cat.Id, new List<string> { "a.jpg", "b.jpg" });
            var handler = new UpdateProductCommandHandler(_unitOfWork, _localizerMock.Object);

            var reordered = new List<string> { "b.jpg", "a.jpg" };
            var result = await handler.Handle(new UpdateProductCommand(
                prod.Id, "P", "م", "D", "و", 10m,
                prod.ImageUrl, reordered, new List<string>(), new List<string>(),
                cat.Id, 1, 0), default);

            result.ImageUrl.Should().Be("b.jpg");
        }

        [Fact]
        public async Task GetProductById_ShouldResolveImagesToFilePaths()
        {
            var cat = await SeedCategory();
            var prod = await SeedProductWithImages(cat.Id, new List<string> { "a.jpg", "b.jpg" });

            var handler = new GetProductByIdQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductByIdQuery(prod.Id), default);

            result.Images.Should().Equal("/files/a.jpg", "/files/b.jpg");
            result.ImageUrl.Should().Be("/files/a.jpg");
        }

        [Fact]
        public async Task GetProductById_WithNullImagesColumn_ShouldReturnEmptyList()
        {
            // Rows written before multi-image support have a null Images column.
            var cat = await SeedCategory();
            var prod = await SeedProductWithImages(cat.Id, new List<string> { "a.jpg" });

            var tracked = await _context.Products.FirstAsync(p => p.Id == prod.Id);
            tracked.Images = null;
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetProductByIdQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductByIdQuery(prod.Id), default);

            result.Images.Should().BeEmpty();
            result.ImageUrl.Should().Be("/files/a.jpg");
        }

        [Fact]
        public async Task GetProducts_ShouldNotPopulateImagesForListCards()
        {
            // List queries deliberately skip the JSON blob so card grids stay cheap.
            var cat = await SeedCategory();
            await SeedProductWithImages(cat.Id, new List<string> { "a.jpg", "b.jpg" });

            var handler = new GetProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetProductsQuery(null, null, 1, 20), default);

            result.Items.Should().HaveCount(1);
            result.Items.First().Images.Should().BeEmpty();
            result.Items.First().ImageUrl.Should().Be("/files/a.jpg");
        }

        [Fact]
        public async Task ToggleProductFeatured_ShouldToggleStatus()
        {
            var cat = await SeedCategory();
            var prod = new Product
            {
                Name = "Featured Test",
                ArName = "اختبار مميز",
                Price = 50m,
                CategoryId = cat.Id,
                QuantityInStock = 5,
                IsFeatured = false
            };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();

            var handler = new ToggleProductFeaturedCommandHandler(_unitOfWork);
            var result1 = await handler.Handle(new ToggleProductFeaturedCommand(prod.Id), default);

            result1.Should().BeTrue();
            var updated = await _context.Products.FindAsync(prod.Id);
            updated!.IsFeatured.Should().BeTrue();

            var result2 = await handler.Handle(new ToggleProductFeaturedCommand(prod.Id), default);
            result2.Should().BeFalse();
            var updated2 = await _context.Products.FindAsync(prod.Id);
            updated2!.IsFeatured.Should().BeFalse();
        }

        [Fact]
        public async Task ToggleProductFeatured_WhenNotFound_ShouldThrowNotFoundException()
        {
            var handler = new ToggleProductFeaturedCommandHandler(_unitOfWork);
            var act = () => handler.Handle(new ToggleProductFeaturedCommand(Guid.NewGuid()), default);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetFeaturedProducts_WhenProductsMarkedAsFeatured_ShouldReturnOnlyFeaturedProducts()
        {
            var cat = await SeedCategory();
            _context.Products.Add(new Product { Name = "Featured 1", ArName = "مميز 1", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1, IsFeatured = true });
            _context.Products.Add(new Product { Name = "Regular", ArName = "عادي", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1, IsFeatured = false });
            await _context.SaveChangesAsync();

            var handler = new GetFeaturedProductsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetFeaturedProductsQuery(), default);

            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Featured 1");
            result.First().IsFeatured.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_WithIsFeaturedTrue_ShouldSetIsFeatured()
        {
            var cat = await SeedCategory();
            var cmd = new CreateProductCommand(
                "Feat", "مميز", "Desc", "وصف", 100m, "img.jpg",
                new List<string> { "img.jpg" }, new List<string>(), new List<string>(),
                cat.Id, 10, 2, IsFeatured: true);

            var handler = new CreateProductCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(cmd, default);

            result.IsFeatured.Should().BeTrue();
            var inDb = await _context.Products.FindAsync(result.Id);
            inDb!.IsFeatured.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WithIsFeaturedTrue_ShouldUpdateIsFeatured()
        {
            var cat = await SeedCategory();
            var prod = new Product { Name = "P", ArName = "م", Price = 10m, CategoryId = cat.Id, QuantityInStock = 1, IsFeatured = false };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();

            var cmd = new UpdateProductCommand(
                prod.Id, "P Updated", "م محدث", "Desc", "وصف", 15m, "img.jpg",
                new List<string> { "img.jpg" }, new List<string>(), new List<string>(),
                cat.Id, 5, 1, IsFeatured: true);

            var handler = new UpdateProductCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(cmd, default);

            result.IsFeatured.Should().BeTrue();
            var inDb = await _context.Products.FindAsync(prod.Id);
            inDb!.IsFeatured.Should().BeTrue();
        }
    }
}
