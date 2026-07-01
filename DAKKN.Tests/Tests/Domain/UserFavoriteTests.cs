using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class UserFavoriteTests
{
    [Fact]
    public void Constructor_ShouldInitializeCompositeKey()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var favorite = new UserFavorite
        {
            UserId = userId,
            ProductId = productId
        };

        favorite.Id.Should().NotBeEmpty();
        favorite.UserId.Should().Be(userId);
        favorite.ProductId.Should().Be(productId);
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var favorite = new UserFavorite();
        favorite.UserId.Should().BeEmpty();
        favorite.ProductId.Should().BeEmpty();
    }
}
