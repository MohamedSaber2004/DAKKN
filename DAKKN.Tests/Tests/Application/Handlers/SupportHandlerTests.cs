using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.Commands.CloseTicket;
using DAKKN.Application.Features.Support.Commands.CreateCategory;
using DAKKN.Application.Features.Support.Commands.CreateFAQ;
using DAKKN.Application.Features.Support.Queries.GetCategories;
using DAKKN.Application.Features.Support.Queries.GetFAQs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class SupportHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Guid _userId;

        public SupportHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _userId = Guid.NewGuid();
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(_userId);
            _currentUserMock.Setup(c => c.UserName).Returns("TestUser");
            _currentUserMock.Setup(c => c.IsAuthenticated).Returns(true);
            _emailServiceMock = new Mock<IEmailService>();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnActiveOnly()
        {
            _context.Set<SupportCategory>().AddRange(
                new SupportCategory { Name = "General", ArName = "عام", IsActive = true },
                new SupportCategory { Name = "Inactive", ArName = "غير نشط", IsActive = false });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetCategoriesQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetCategoriesQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCategories_IncludeInactive_ShouldReturnAll()
        {
            _context.Set<SupportCategory>().AddRange(
                new SupportCategory { Name = "General", ArName = "عام", IsActive = true },
                new SupportCategory { Name = "Inactive", ArName = "غير نشط", IsActive = false });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetCategoriesQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetCategoriesQuery(IncludeInactive: true), default);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnDto()
        {
            var handler = new CreateCategoryCommandHandler(_unitOfWork, _currentUserMock.Object);
            var result = await handler.Handle(new CreateCategoryCommand
            {
                Name = "Orders",
                ArName = "الطلبات",
                Description = "Order support",
                Icon = "box",
                DisplayOrder = 1
            }, default);

            result.Should().NotBeNull();
            result.Name.Should().Be("Orders");
        }

        [Fact]
        public async Task GetFAQs_ShouldReturnPublished()
        {
            var cat = new SupportCategory { Name = "General", ArName = "عام", IsActive = true };
            _context.Set<SupportCategory>().Add(cat);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            _context.Set<SupportFAQ>().AddRange(
                new SupportFAQ { Question = "Q1", ArQuestion = "س1", Answer = "A1", ArAnswer = "ج1", CategoryId = cat.Id, IsPublished = true },
                new SupportFAQ { Question = "Q2", ArQuestion = "س2", Answer = "A2", ArAnswer = "ج2", CategoryId = cat.Id, IsPublished = false });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new GetFAQsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetFAQsQuery(OnlyPublished: true), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateFAQ_ShouldReturnDto()
        {
            var cat = new SupportCategory { Name = "General", ArName = "عام", IsActive = true };
            _context.Set<SupportCategory>().Add(cat);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new CreateFAQCommandHandler(_unitOfWork, _currentUserMock.Object);
            var result = await handler.Handle(new CreateFAQCommand
            {
                Question = "How?",
                ArQuestion = "كيف؟",
                Answer = "Like this",
                ArAnswer = "هكذا",
                CategoryId = cat.Id,
                DisplayOrder = 1
            }, default);

            result.Should().NotBeNull();
            result.Question.Should().Be("How?");
        }

        [Fact]
        public async Task CloseTicket_ShouldSucceed()
        {
            var ticket = SupportTicket.Create(_userId, "Customer", "cust@test.com", "Issue", "Help",
                Guid.Empty, SupportTicketPriority.Medium, null, "web", null);
            _context.Set<SupportTicket>().Add(ticket);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new CloseTicketCommandHandler(_unitOfWork, _currentUserMock.Object, _emailServiceMock.Object);
            var result = await handler.Handle(new CloseTicketCommand(ticket.Id), default);

            result.Should().BeTrue();
            var updated = _context.Set<SupportTicket>().Find(ticket.Id);
            updated!.Status.Should().Be(SupportTicketStatus.Closed);
        }
    }
}
