using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion;
using DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetAllSuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class StickerSuggestionHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IImageValidator> _imageValidatorMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Guid _userId;
        private readonly Guid _adminId;

        public StickerSuggestionHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _imageValidatorMock = new Mock<IImageValidator>();
            _userId = Guid.NewGuid();
            _adminId = Guid.NewGuid();
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(_userId);
            _currentUserMock.Setup(c => c.IsAuthenticated).Returns(true);
            _currentUserMock.Setup(c => c.UserType).Returns(UserType.User);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task SubmitSuggestion_WithoutImage_ShouldCreate()
        {
            var handler = new SubmitSuggestionCommandHandler(_unitOfWork, _currentUserMock.Object, _imageValidatorMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new SubmitSuggestionCommand
            {
                Title = "New Sticker",
                Description = "Cool design",
                Tags = "fun, cool"
            }, default);

            result.Should().NotBeNull();
            result.Title.Should().Be("New Sticker");
        }

        [Fact]
        public async Task SubmitSuggestion_WithImage_ShouldUpload()
        {
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), 6))
                .ReturnsAsync((true, "/uploads/sticker.jpg"));

            var handler = new SubmitSuggestionCommandHandler(_unitOfWork, _currentUserMock.Object, _imageValidatorMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new SubmitSuggestionCommand
            {
                Title = "Image Sticker",
                Description = "With image",
                ReferenceImage = Mock.Of<IFormFile>()
            }, default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task SubmitSuggestion_ImageUploadFails_ShouldThrow()
        {
            _imageValidatorMock.Setup(v => v.UploadImage(It.IsAny<IFormFile>(), 6))
                .ReturnsAsync((false, "Failed"));

            var handler = new SubmitSuggestionCommandHandler(_unitOfWork, _currentUserMock.Object, _imageValidatorMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new SubmitSuggestionCommand
            {
                Title = "Fail",
                Description = "Fail",
                ReferenceImage = Mock.Of<IFormFile>()
            }, default)).Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task UpdateSuggestionStatus_UnderReview_ShouldSucceed()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            _context.Users.Add(user);
            var suggestion = StickerSuggestion.Create(_userId, "Title", "Desc", null, null);
            _context.Set<StickerSuggestion>().Add(suggestion);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            _currentUserMock.Setup(c => c.UserId).Returns(_adminId);

            var handler = new UpdateSuggestionStatusCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new UpdateSuggestionStatusCommand
            {
                SuggestionId = suggestion.Id,
                NewStatus = SuggestionStatus.UnderReview
            }, default);

            var updated = _context.Set<StickerSuggestion>().Find(suggestion.Id);
            updated!.Status.Should().Be(SuggestionStatus.UnderReview);
        }

        [Fact]
        public async Task UpdateSuggestionStatus_RejectedThenApproved_ShouldThrow()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            _context.Users.Add(user);
            var suggestion = StickerSuggestion.Create(_userId, "Title", "Desc", null, null);
            _context.Set<StickerSuggestion>().Add(suggestion);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            _currentUserMock.Setup(c => c.UserId).Returns(_adminId);

            var handler = new UpdateSuggestionStatusCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, NewStatus = SuggestionStatus.Rejected }, default);
            await FluentActions.Invoking(() => handler.Handle(new UpdateSuggestionStatusCommand { SuggestionId = suggestion.Id, NewStatus = SuggestionStatus.Approved }, default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task GetAllSuggestions_ShouldReturnPaginated()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            user.Id = _userId;
            _context.Users.Add(user);
            var sug = StickerSuggestion.Create(_userId, "T", "D", null, null);
            _context.Set<StickerSuggestion>().Add(sug);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetAllSuggestionsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetAllSuggestionsQuery(), default);

            result.Items.Should().HaveCount(1);
            result.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task GetMySuggestions_ShouldReturnForCurrentUser()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            user.Id = _userId;
            _context.Users.Add(user);
            var sug1 = StickerSuggestion.Create(_userId, "Mine", "Desc", null, null);
            var sug2 = StickerSuggestion.Create(Guid.NewGuid(), "Not mine", "Desc", null, null);
            _context.Set<StickerSuggestion>().AddRange(sug1, sug2);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetMySuggestionsQueryHandler(_unitOfWork, _currentUserMock.Object);
            var result = await handler.Handle(new GetMySuggestionsQuery(), default);

            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetSuggestionById_AsOwner_ShouldReturn()
        {
            var user = ApplicationUser.Create("User", "user@test.com", "1234567890");
            user.Id = _userId;
            _context.Users.Add(user);
            var sug = StickerSuggestion.Create(_userId, "My Suggestion", "Desc", null, null);
            _context.Set<StickerSuggestion>().Add(sug);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetSuggestionByIdQueryHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new GetSuggestionByIdQuery(sug.Id), default);

            result.Should().NotBeNull();
            result.Title.Should().Be("My Suggestion");
        }

        [Fact]
        public async Task GetSuggestionById_AsAdmin_ShouldReturn()
        {
            var ownerId = Guid.NewGuid();
            var owner = ApplicationUser.Create("Owner", "owner@test.com", "1111111111");
            owner.Id = ownerId;
            var admin = ApplicationUser.Create("Admin", "admin@test.com", "2222222222");
            admin.Id = _adminId;
            _context.Users.AddRange(owner, admin);
            var sug = StickerSuggestion.Create(ownerId, "Admin View", "Desc", null, null);
            _context.Set<StickerSuggestion>().Add(sug);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            _currentUserMock.Setup(c => c.UserId).Returns(_adminId);
            _currentUserMock.Setup(c => c.UserType).Returns(UserType.Admin);

            var handler = new GetSuggestionByIdQueryHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new GetSuggestionByIdQuery(sug.Id), default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetSuggestionById_UnauthorizedUser_ShouldThrow()
        {
            var ownerId = Guid.NewGuid();
            var owner = ApplicationUser.Create("Owner", "owner@test.com", "1111111111");
            owner.Id = ownerId;
            _context.Users.Add(owner);
            var sug = StickerSuggestion.Create(ownerId, "Not Mine", "Desc", null, null);
            _context.Set<StickerSuggestion>().Add(sug);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetSuggestionByIdQueryHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new GetSuggestionByIdQuery(sug.Id), default))
                .Should().ThrowAsync<UnAuthorizedException>();
        }
    }
}
