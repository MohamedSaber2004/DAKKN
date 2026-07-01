using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.AccountSecurity.Commands.ChangePassword;
using DAKKN.Application.Features.AccountSecurity.Commands.DeleteAccount;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class AccountSecurityHandlerTests
    {
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ILogger<ChangePasswordCommandHandler>> _changePasswordLoggerMock;
        private readonly Mock<ILogger<DeleteAccountCommandHandler>> _deleteAccountLoggerMock;
        private readonly Guid _userId;

        public AccountSecurityHandlerTests()
        {
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _userId = Guid.NewGuid();
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(_userId);
            _currentUserMock.Setup(c => c.IsAuthenticated).Returns(true);
            _changePasswordLoggerMock = new Mock<ILogger<ChangePasswordCommandHandler>>();
            _deleteAccountLoggerMock = new Mock<ILogger<DeleteAccountCommandHandler>>();
        }

        [Fact]
        public async Task ChangePassword_ShouldSucceed()
        {
            var user = new ApplicationUser("user", "user@test.com", "User", DateTime.UtcNow, Gender.Male) { Id = _userId };
            var mockUserManager = CreateMockUserManager(user);
            mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockUserManager.Setup(m => m.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new ChangePasswordCommandHandler(mockUserManager.Object, _currentUserMock.Object, _localizerMock.Object, _changePasswordLoggerMock.Object);
            await handler.Handle(new ChangePasswordCommand("OldPass1!", "NewPass1!", "NewPass1!"), default);
        }

        [Fact]
        public async Task ChangePassword_WrongCurrentPassword_ShouldThrow()
        {
            var user = new ApplicationUser("user", "user@test.com", "User", DateTime.UtcNow, Gender.Male) { Id = _userId };
            var mockUserManager = CreateMockUserManager(user);
            mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new ChangePasswordCommandHandler(mockUserManager.Object, _currentUserMock.Object, _localizerMock.Object, _changePasswordLoggerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new ChangePasswordCommand("WrongPass1!", "NewPass1!", "NewPass1!"), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task ChangePassword_UserNotFound_ShouldThrow()
        {
            var mockUserManager = CreateMockUserManager(null);
            var handler = new ChangePasswordCommandHandler(mockUserManager.Object, _currentUserMock.Object, _localizerMock.Object, _changePasswordLoggerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new ChangePasswordCommand("Old1!", "New1!", "New1!"), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteAccount_ShouldSucceed()
        {
            var user = new ApplicationUser("user", "user@test.com", "User", DateTime.UtcNow, Gender.Male) { Id = _userId };
            var mockUserManager = CreateMockUserManager(user);
            mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var mockSignInManager = CreateMockSignInManager();

            var handler = new DeleteAccountCommandHandler(mockUserManager.Object, mockSignInManager.Object, _currentUserMock.Object, _localizerMock.Object, _deleteAccountLoggerMock.Object);
            await handler.Handle(new DeleteAccountCommand("Pass1!", "DELETE"), default);

            user.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAccount_WrongConfirmation_ShouldThrow()
        {
            var user = new ApplicationUser("user", "user@test.com", "User", DateTime.UtcNow, Gender.Male) { Id = _userId };
            var mockUserManager = CreateMockUserManager(user);
            var mockSignInManager = CreateMockSignInManager();

            var handler = new DeleteAccountCommandHandler(mockUserManager.Object, mockSignInManager.Object, _currentUserMock.Object, _localizerMock.Object, _deleteAccountLoggerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteAccountCommand("Pass1!", "NOTDELETE"), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task DeleteAccount_WrongPassword_ShouldThrow()
        {
            var user = new ApplicationUser("user", "user@test.com", "User", DateTime.UtcNow, Gender.Male) { Id = _userId };
            var mockUserManager = CreateMockUserManager(user);
            mockUserManager.Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);
            var mockSignInManager = CreateMockSignInManager();

            var handler = new DeleteAccountCommandHandler(mockUserManager.Object, mockSignInManager.Object, _currentUserMock.Object, _localizerMock.Object, _deleteAccountLoggerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteAccountCommand("Wrong1!", "DELETE"), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager(ApplicationUser? user)
        {
            var mgr = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            mgr.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            return mgr;
        }

        private static Mock<SignInManager<ApplicationUser>> CreateMockSignInManager()
        {
            var userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
                Mock.Of<IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<ApplicationUser>>());
        }
    }
}
