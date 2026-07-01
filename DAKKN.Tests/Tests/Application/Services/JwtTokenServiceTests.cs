using DAKKN.Application.Common.Options;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace DAKKN.Tests.Tests.Application.Services
{
    public class JwtTokenServiceTests
    {
        private readonly JwtSettings _settings;
        private readonly JwtTokenService _service;

        public JwtTokenServiceTests()
        {
            _settings = new JwtSettings
            {
                Secret = "ThisIsASuperSecretKeyForTesting12345678!",
                Issuer = "DAKKN",
                Audience = "DAKKN_Users",
                ExpiryInMinutes = 60,
                RefreshTokenExpiryDays = 7
            };

            var optionsMock = new Mock<IOptions<JwtSettings>>();
            optionsMock.Setup(o => o.Value).Returns(_settings);
            _service = new JwtTokenService(optionsMock.Object);
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnValidJwt()
        {
            var user = new ApplicationUser("testuser", "test@dakkn.com", "Test User", new DateTime(1990, 1, 1), Gender.Male);
            var roles = new List<string> { "User" };

            var token = _service.GenerateAccessToken(user, roles);

            token.Should().NotBeNullOrEmpty();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Should().NotBeNull();
            jwtToken.Subject.Should().Be(user.Id.ToString());
            jwtToken.Claims.Should().Contain(c => c.Type == "FullName" && c.Value == "Test User");
            jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
            jwtToken.Issuer.Should().Be("DAKKN");
            jwtToken.Audiences.Should().Contain("DAKKN_Users");
        }

        [Fact]
        public void GenerateAccessToken_ShouldIncludeMultipleRoles()
        {
            var user = new ApplicationUser("adminuser", "admin@dakkn.com", "Admin User", new DateTime(1985, 5, 15), Gender.Male);
            var roles = new List<string> { "Admin", "User" };

            var token = _service.GenerateAccessToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
            roleClaims.Should().BeEquivalentTo(new[] { "Admin", "User" });
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidJwt()
        {
            var user = new ApplicationUser("testuser", "test@dakkn.com", "Test User", new DateTime(1990, 1, 1), Gender.Male);

            var token = _service.GenerateRefreshToken(user);

            token.Should().NotBeNullOrEmpty();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == "TokenType" && c.Value == "RefreshToken");
        }
    }
}
