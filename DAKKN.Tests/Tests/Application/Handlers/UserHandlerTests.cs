using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Users.Queries.ExportUsers;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using DAKKN.Application.Features.Users.Queries.GetUserStats;
using DAKKN.Application.Features.Users.Queries.GetUserById;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using DAKKN.Application.Features.Users.Commands.DeleteUser;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class UserHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Guid _userId;

        public UserHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _userId = Guid.NewGuid();
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(_userId);
            _currentUserMock.Setup(c => c.IsAuthenticated).Returns(true);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnPaginated()
        {
            var user1 = ApplicationUser.Create("User One", "user1@test.com", "1111111111");
            user1.Id = Guid.NewGuid();
            var user2 = ApplicationUser.Create("User Two", "user2@test.com", "2222222222");
            user2.Id = Guid.NewGuid();
            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetAllUsersQueryHandler(_context);
            var result = await handler.Handle(new GetAllUsersQuery(), default);

            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllUsers_WithSearch_ShouldFilter()
        {
            var user1 = ApplicationUser.Create("John Doe", "john@test.com", "1111111111");
            user1.Id = Guid.NewGuid();
            var user2 = ApplicationUser.Create("Jane Smith", "jane@test.com", "2222222222");
            user2.Id = Guid.NewGuid();
            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetAllUsersQueryHandler(_context);
            var result = await handler.Handle(new GetAllUsersQuery(SearchTerm: "John"), default);

            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExportUsers_ShouldReturnAll()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ExportUsersQueryHandler(_context);
            var result = await handler.Handle(new ExportUsersQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetUserSettings_ShouldReturnDto()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            user.Id = _userId;
            _context.Users.Add(user);
            _context.Set<UserSettings>().Add(new UserSettings
            {
                UserId = _userId,
                Language = "ar",
                Theme = "dark",
                LayoutMode = "default"
            });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var mockUserManager = CreateMockUserManager(user);

            var handler = new GetUserSettingsQueryHandler(_unitOfWork, _currentUserMock.Object, mockUserManager.Object);
            var result = await handler.Handle(new GetUserSettingsQuery(), default);

            result.Should().NotBeNull();
            result.FullName.Should().Be("User");
            result.Theme.Should().Be("dark");
        }



        [Fact]
        public async Task DeleteUser_ShouldSucceed()
        {
            var adminUser = ApplicationUser.Create("Admin", "admin@test.com", "0000000000");
            adminUser.Id = _userId;
            var targetUser = ApplicationUser.Create("Target", "target@test.com", "1111111111");
            targetUser.Id = Guid.NewGuid();
            _context.Users.AddRange(adminUser, targetUser);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => targetUser);
            mockUserManager.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var handler = new DeleteUserCommandHandler(mockUserManager.Object, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new DeleteUserCommand(targetUser.Id), default);

            targetUser.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUser_OwnAccount_ShouldThrow()
        {
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new ApplicationUser("self", "self@test.com", "Self", DateTime.UtcNow, Gender.Male) { Id = _userId });

            var handler = new DeleteUserCommandHandler(mockUserManager.Object, _currentUserMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteUserCommand(_userId), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        private static Mock<UserManager<ApplicationUser>> CreateMockUserManager(ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            var users = new List<ApplicationUser> { user }.AsQueryable();
            mgr.Setup(m => m.Users).Returns(users);
            mgr.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => users.FirstOrDefault(u => u.Id.ToString() == id));
            mgr.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>());
            mgr.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            return mgr;
        }
    }
}
