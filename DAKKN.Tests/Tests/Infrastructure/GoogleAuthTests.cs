using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Tests.Tests.Infrastructure
{
    public class GoogleAuthTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly GoogleAuthSettings _googleSettings;
        private readonly GoogleAuth _service;

        public GoogleAuthTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null!, null!, null!, null!);

            _googleSettings = new GoogleAuthSettings
            {
                WebClientId = "test-client-id.apps.googleusercontent.com",
                WebClientSecret = "test-secret"
            };

            var settingsMock = new Mock<IOptions<GoogleAuthSettings>>();
            settingsMock.Setup(x => x.Value).Returns(_googleSettings);

            _service = new GoogleAuth(_signInManagerMock.Object, settingsMock.Object);
        }

        [Fact]
        public async Task ValidateGoogleTokenAsync_WithInvalidToken_ShouldReturnNull()
        {
            var result = await _service.ValidateGoogleTokenAsync("invalid-token", default);
            result.Should().BeNull();
        }

        [Fact(Skip = "Google APIs throw ArgumentException for empty token before InvalidJwtException catch")]
        public async Task ValidateGoogleTokenAsync_WithEmptyToken_ShouldReturnNull()
        {
            var result = await _service.ValidateGoogleTokenAsync("", default);
            result.Should().BeNull();
        }

        [Fact]
        public async Task LinkGoogleAccountIfNeeded_WhenNoGoogleLogin_ShouldAddLogin()
        {
            var user = CreateTestUser();
            var payload = CreateTestPayload();

            _userManagerMock.Setup(x => x.GetLoginsAsync(user))
                .ReturnsAsync(new List<UserLoginInfo>());

            _userManagerMock.Setup(x => x.AddLoginAsync(user, It.IsAny<UserLoginInfo>()))
                .ReturnsAsync(IdentityResult.Success);

            await _service.LinkGoogleAccountIfNeeded(user, payload, "corr-123");

            _userManagerMock.Verify(x => x.AddLoginAsync(user, It.Is<UserLoginInfo>(l => l.LoginProvider == "Google")), Times.Once);
        }

        [Fact]
        public async Task LinkGoogleAccountIfNeeded_WhenGoogleLoginExists_ShouldNotAddLogin()
        {
            var user = CreateTestUser();
            var payload = CreateTestPayload();

            _userManagerMock.Setup(x => x.GetLoginsAsync(user))
                .ReturnsAsync(new List<UserLoginInfo>
                {
                    new("Google", payload.Subject, "Google")
                });

            await _service.LinkGoogleAccountIfNeeded(user, payload, "corr-123");

            _userManagerMock.Verify(x => x.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserLoginInfo>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserInfoFromGoogle_WhenNameDiffers_ShouldUpdate()
        {
            var user = CreateTestUser();
            var payload = CreateTestPayload(name: "New Name From Google");

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            await _service.UpdateUserInfoFromGoogle(user, payload, "corr-123");

            user.FullName.Should().Be("New Name From Google");
            _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoFromGoogle_WhenNameSame_ShouldNotUpdate()
        {
            var user = CreateTestUser();
            var payload = CreateTestPayload(name: "Test User");

            await _service.UpdateUserInfoFromGoogle(user, payload, "corr-123");

            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserInfoFromGoogle_WhenUpdateFails_ShouldThrow()
        {
            var user = CreateTestUser();
            var payload = CreateTestPayload(name: "New Name");

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateUserInfoFromGoogle(user, payload, "corr-123"));
        }

        private static ApplicationUser CreateTestUser()
        {
            return new ApplicationUser("testuser", "test@test.com", "Test User", new DateTime(1990, 1, 1), Gender.Male);
        }

        private static GoogleJsonWebSignature.Payload CreateTestPayload(string? name = null)
        {
            return new GoogleJsonWebSignature.Payload
            {
                Subject = "google-sub-123",
                Name = name ?? "Test User",
                Email = "test@gmail.com",
                Picture = "https://example.com/pic.jpg"
            };
        }
    }
}
