using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Favorites.Commands.RemoveFavorite;
using DAKKN.Application.Features.Favorites.Commands.ToggleFavorite;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class FavoritesHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Guid _userId = Guid.NewGuid();

        public FavoritesHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(x => x.UserId).Returns(_userId);
            _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task ToggleFavorite_ShouldAddNew()
        {
            var handler = new ToggleFavoriteCommandHandler(_unitOfWork, _currentUserMock.Object);
            var added = await handler.Handle(new ToggleFavoriteCommand(Guid.NewGuid()), default);

            added.Should().BeTrue();
            _context.UserFavorites.Count().Should().Be(1);
        }

        [Fact]
        public async Task ToggleFavorite_ShouldRemoveExisting()
        {
            var pid = Guid.NewGuid();
            _context.UserFavorites.Add(new UserFavorite { UserId = _userId, ProductId = pid, IsDeleted = false });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ToggleFavoriteCommandHandler(_unitOfWork, _currentUserMock.Object);
            var removed = await handler.Handle(new ToggleFavoriteCommand(pid), default);

            removed.Should().BeFalse();
        }

        [Fact]
        public async Task RemoveFavorite_ShouldRemove()
        {
            var pid = Guid.NewGuid();
            _context.UserFavorites.Add(new UserFavorite { UserId = _userId, ProductId = pid, IsDeleted = false });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new RemoveFavoriteCommandHandler(_unitOfWork, _currentUserMock.Object);
            await handler.Handle(new RemoveFavoriteCommand(pid), default);

            _context.UserFavorites.Count().Should().Be(0);
        }

        [Fact]
        public async Task RemoveFavorite_NotFound_ShouldThrow()
        {
            var handler = new RemoveFavoriteCommandHandler(_unitOfWork, _currentUserMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new RemoveFavoriteCommand(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }
    }
}
