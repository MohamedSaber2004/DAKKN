using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.BrandReviews.Commands.ApproveBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.CreateBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.RejectBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.ToggleDisplayBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.UpdateBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.UpdateDisplayOrderBrandReview;
using DAKKN.Application.Features.BrandReviews.Queries.GetAdminBrandReviews;
using DAKKN.Application.Features.BrandReviews.Queries.GetCustomerBrandReviews;
using DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews;
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
    public class BrandReviewHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;

        public BrandReviewHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateBrandReview_ShouldReturnDto()
        {
            var handler = new CreateBrandReviewCommandHandler(_unitOfWork);
            var result = await handler.Handle(new CreateBrandReviewCommand
            {
                UserId = Guid.NewGuid(),
                Rating = 5,
                ReviewTitle = "Great",
                ReviewText = "Excellent product!"
            }, default);

            result.Should().NotBeNull();
            result.Rating.Should().Be(5);
        }

        [Fact]
        public async Task ApproveBrandReview_ShouldSucceed()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 4, ReviewTitle = "T", ReviewText = "D" };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ApproveBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await handler.Handle(new ApproveBrandReviewCommand(review.Id, Guid.NewGuid()), default);

            var updated = await _context.BrandReviews.FindAsync(review.Id);
            updated!.IsApproved.Should().BeTrue();
        }

        [Fact]
        public async Task ApproveBrandReview_NotFound_ShouldThrow()
        {
            var handler = new ApproveBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new ApproveBrandReviewCommand(Guid.NewGuid(), Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RejectBrandReview_ShouldSucceed()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 2, ReviewTitle = "T", ReviewText = "D" };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new RejectBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await handler.Handle(new RejectBrandReviewCommand(review.Id), default);

            var updated = await _context.BrandReviews.FindAsync(review.Id);
            updated!.IsApproved.Should().BeFalse();
        }

        [Fact]
        public async Task ToggleDisplayBrandReview_ShouldToggle()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 3, ReviewTitle = "T", ReviewText = "D", IsDisplayed = false, IsApproved = true };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ToggleDisplayBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await handler.Handle(new ToggleDisplayBrandReviewCommand(review.Id), default);

            var updated = await _context.BrandReviews.FindAsync(review.Id);
            updated!.IsDisplayed.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateDisplayOrderBrandReview_ShouldUpdate()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 3, ReviewTitle = "T", ReviewText = "D" };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new UpdateDisplayOrderBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await handler.Handle(new UpdateDisplayOrderBrandReviewCommand(review.Id, 5), default);

            var updated = await _context.BrandReviews.FindAsync(review.Id);
            updated!.DisplayOrder.Should().Be(5);
        }

        [Fact]
        public async Task DeleteBrandReview_ShouldSucceed()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 3, ReviewTitle = "T", ReviewText = "D" };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new DeleteBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            await handler.Handle(new DeleteBrandReviewCommand(review.Id, review.UserId), default);

            var deleted = _context.BrandReviews.IgnoreQueryFilters().FirstOrDefault(r => r.Id == review.Id);
            deleted.Should().NotBeNull();
            deleted!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateBrandReview_ShouldReturnDto()
        {
            var review = new BrandReview { UserId = Guid.NewGuid(), Rating = 3, ReviewTitle = "Old", ReviewText = "Old" };
            _context.BrandReviews.Add(review);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new UpdateBrandReviewCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(new UpdateBrandReviewCommand(review.Id, review.UserId, 5, "New", "New text"), default);

            result.ReviewTitle.Should().Be("New");
        }

        [Fact]
        public async Task GetDisplayedBrandReviews_ShouldReturnDisplayed()
        {
            var user1 = ApplicationUser.Create("User1", "user1@test.com", "1234567890");
            var user2 = ApplicationUser.Create("User2", "user2@test.com", "0987654321");
            _context.Users.AddRange(user1, user2);
            _context.BrandReviews.AddRange(
                new BrandReview { UserId = user1.Id, Rating = 5, ReviewTitle = "A", ReviewText = "D", IsDisplayed = true, IsApproved = true },
                new BrandReview { UserId = user2.Id, Rating = 3, ReviewTitle = "B", ReviewText = "D", IsDisplayed = false, IsApproved = true });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetDisplayedBrandReviewsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetDisplayedBrandReviewsQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCustomerBrandReviews_ShouldReturnForUser()
        {
            var user1 = ApplicationUser.Create("User1", "user1@test.com", "1234567890");
            var user2 = ApplicationUser.Create("User2", "user2@test.com", "0987654321");
            _context.Users.AddRange(user1, user2);
            _context.BrandReviews.AddRange(
                new BrandReview { UserId = user1.Id, Rating = 5, ReviewTitle = "A", ReviewText = "D" },
                new BrandReview { UserId = user2.Id, Rating = 3, ReviewTitle = "B", ReviewText = "D" });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetCustomerBrandReviewsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetCustomerBrandReviewsQuery(user1.Id), default);

            result.Should().HaveCount(1);
        }
    }
}
