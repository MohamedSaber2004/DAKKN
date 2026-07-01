using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class BrandReviewTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var review = new BrandReview
        {
            UserId = Guid.NewGuid(),
            Rating = 4,
            ReviewTitle = "Great product",
            ReviewText = "Really enjoyed using this.",
            IsApproved = true,
            IsDisplayed = true,
            DisplayOrder = 1,
            ApprovedBy = Guid.NewGuid(),
            ApprovedAt = DateTime.UtcNow
        };

        review.Id.Should().NotBeEmpty();
        review.UserId.Should().NotBeEmpty();
        review.Rating.Should().Be(4);
        review.ReviewTitle.Should().Be("Great product");
        review.ReviewText.Should().Be("Really enjoyed using this.");
        review.IsApproved.Should().BeTrue();
        review.IsDisplayed.Should().BeTrue();
        review.DisplayOrder.Should().Be(1);
        review.ApprovedBy.Should().NotBeNull();
        review.ApprovedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DefaultValues_ShouldBeEmptyStrings()
    {
        var review = new BrandReview();
        review.ReviewTitle.Should().BeEmpty();
        review.ReviewText.Should().BeEmpty();
        review.Rating.Should().Be(0);
        review.IsApproved.Should().BeFalse();
        review.IsDisplayed.Should().BeFalse();
    }
}
