using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class ProductRatingTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var rating = new ProductRating
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Stars = 4
        };

        rating.Id.Should().NotBeEmpty();
        rating.ProductId.Should().NotBeEmpty();
        rating.UserId.Should().NotBeEmpty();
        rating.Stars.Should().Be(4);
    }

    [Fact]
    public void Stars_ShouldBeOne_WhenMinimumRating()
    {
        var rating = new ProductRating { Stars = 1 };
        rating.Stars.Should().Be(1);
    }

    [Fact]
    public void Stars_ShouldBeFive_WhenMaximumRating()
    {
        var rating = new ProductRating { Stars = 5 };
        rating.Stars.Should().Be(5);
    }

    [Fact]
    public void Stars_ShouldAllowAllIntermediateValues()
    {
        var rating = new ProductRating { Stars = 3 };
        rating.Stars.Should().Be(3);
    }
}
