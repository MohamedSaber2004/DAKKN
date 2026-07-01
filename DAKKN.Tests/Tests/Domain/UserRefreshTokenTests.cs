using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class UserRefreshTokenTests
{
    [Fact]
    public void Create_ShouldInitializeToken()
    {
        var userId = Guid.NewGuid();
        var expiry = DateTime.UtcNow.AddDays(7);

        var token = UserRefreshToken.Create(userId, "refresh-token-value", expiry);

        token.UserId.Should().Be(userId);
        token.Token.Should().Be("refresh-token-value");
        token.ExpiryDate.Should().Be(expiry);
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldBeTrue_WhenNotRevokedAndNotExpired()
    {
        var token = UserRefreshToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        token.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_ShouldBeFalse_WhenRevoked()
    {
        var token = UserRefreshToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldBeFalse_WhenExpired()
    {
        var token = UserRefreshToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(-1));
        token.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Revoke_ShouldSetIsRevoked()
    {
        var token = UserRefreshToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Revoke_ShouldMakeTokenInvalid()
    {
        var token = UserRefreshToken.Create(Guid.NewGuid(), "token", DateTime.UtcNow.AddDays(1));
        token.Revoke();

        token.IsValid.Should().BeFalse();
    }
}
