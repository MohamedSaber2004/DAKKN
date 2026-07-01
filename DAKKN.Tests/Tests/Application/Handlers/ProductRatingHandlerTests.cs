using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.ProductRatings.Commands.RateProduct;
using DAKKN.Application.Features.ProductRatings.Queries.GetProductRatingSummary;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class ProductRatingHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Guid _userId;

        public ProductRatingHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _userId = Guid.NewGuid();
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(_userId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task RateProduct_ShouldCreateRating()
        {
            var product = new Product { Name = "Test", ArName = "اختبار" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new RateProductCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new RateProductCommand { ProductId = product.Id, Stars = 5 }, default);

            result.AverageStars.Should().Be(5);
            result.TotalRatings.Should().Be(1);
            result.CurrentUserStars.Should().Be(5);
        }

        [Fact]
        public async Task RateProduct_ShouldUpdateExisting()
        {
            var product = new Product { Name = "Test", ArName = "اختبار" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var rating = new ProductRating { ProductId = product.Id, UserId = _userId, Stars = 3 };
            _context.Set<ProductRating>().Add(rating);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new RateProductCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new RateProductCommand { ProductId = product.Id, Stars = 5 }, default);

            result.AverageStars.Should().Be(5);
            result.CurrentUserStars.Should().Be(5);
        }

        [Fact]
        public async Task RateProduct_ProductNotFound_ShouldThrow()
        {
            var handler = new RateProductCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new RateProductCommand { ProductId = Guid.NewGuid(), Stars = 5 }, default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task GetProductRatingSummary_ShouldReturnSummary()
        {
            var product = new Product { Name = "Test", ArName = "اختبار" };
            _context.Products.Add(product);
            _context.Set<ProductRating>().AddRange(
                new ProductRating { ProductId = product.Id, UserId = _userId, Stars = 4 },
                new ProductRating { ProductId = product.Id, UserId = Guid.NewGuid(), Stars = 2 });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            _currentUserMock.Setup(c => c.IsAuthenticated).Returns(true);

            var handler = new GetProductRatingSummaryQueryHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new GetProductRatingSummaryQuery { ProductId = product.Id }, default);

            result.AverageStars.Should().Be(3);
            result.TotalRatings.Should().Be(2);
            result.CurrentUserStars.Should().Be(4);
        }

        [Fact]
        public async Task GetProductRatingSummary_ProductNotFound_ShouldThrow()
        {
            var handler = new GetProductRatingSummaryQueryHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new GetProductRatingSummaryQuery { ProductId = Guid.NewGuid() }, default))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
