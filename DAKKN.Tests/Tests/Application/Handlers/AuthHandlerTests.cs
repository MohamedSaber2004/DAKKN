using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Features.Auth.Comands.SignIn;
using DAKKN.Application.Features.Auth.Comands.SignUp;
using DAKKN.Application.Features.Auth.DTOs;
using DAKKN.Application.Features.Auth.Queries.CheckGoogleAccount;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class AuthHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
        private readonly Mock<IJwtTokenService> _jwtMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly Mock<IGenericRepository<UserRefreshToken>> _tokenRepoMock;
        private readonly JwtSettings _jwtSettings;

        public AuthHandlerTests()
        {
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                roleStoreMock.Object, null!, null!, null!, null!);

            _jwtMock = new Mock<IJwtTokenService>();
            _jwtMock.Setup(x => x.GenerateAccessToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                .Returns("fake-access-token");
            _jwtMock.Setup(x => x.GenerateRefreshToken(It.IsAny<ApplicationUser>()))
                .Returns("fake-refresh-token");

            _jwtSettings = new JwtSettings
            {
                Secret = "ThisIsASuperSecretKeyForJwtToken12345678!",
                Issuer = "test",
                Audience = "test",
                ExpiryInMinutes = 15,
                RefreshTokenExpiryDays = 7
            };
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenRepoMock = new Mock<IGenericRepository<UserRefreshToken>>();
            _unitOfWorkMock.Setup(u => u.GetRepository<UserRefreshToken>()).Returns(_tokenRepoMock.Object);

            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
        }

        [Fact]
        public async Task SignIn_ShouldReturnAuthResponse()
        {
            var user = ApplicationUser.Create("John Doe", "john@test.com", "1234567890");
            _userManagerMock.Setup(x => x.FindByEmailAsync("john@test.com")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _tokenRepoMock.Setup(r => r.GetFirstAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UserRefreshToken, bool>>>(), default))
                .ReturnsAsync((UserRefreshToken)null!);

            var handler = new SignInCommandHandler(_userManagerMock.Object, _jwtMock.Object, _jwtSettingsMock.Object, _unitOfWorkMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new SignInCommand("john@test.com", "Password123!"), default);

            result.Should().NotBeNull();
            result.AccessToken.Should().Be("fake-access-token");
            result.FullName.Should().Be("John Doe");
        }

        [Fact]
        public async Task SignIn_InvalidCredentials_ShouldThrow()
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync("wrong@test.com")).ReturnsAsync((ApplicationUser)null!);

            var handler = new SignInCommandHandler(_userManagerMock.Object, _jwtMock.Object, _jwtSettingsMock.Object, _unitOfWorkMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new SignInCommand("wrong@test.com", "wrong"), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task SignUp_ShouldCreateUser()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Password123!"))
                .ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });

            var handler = new SignupCommandHandler(_userManagerMock.Object, _roleManagerMock.Object, _jwtMock.Object, _jwtSettingsMock.Object, _unitOfWorkMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new SignupCommand("John Doe", "john@test.com", "1234567890", "Password123!", "Password123!"), default);

            result.Should().NotBeNull();
            result.AccessToken.Should().Be("fake-access-token");
        }

        [Fact]
        public async Task SignUp_CreateFails_ShouldThrow()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Password123!"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            var handler = new SignupCommandHandler(_userManagerMock.Object, _roleManagerMock.Object, _jwtMock.Object, _jwtSettingsMock.Object, _unitOfWorkMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new SignupCommand("John Doe", "john@test.com", "1234567890", "Password123!", "Password123!"), default))
                .Should().ThrowAsync<BadRequestException>();
        }

    }
}
