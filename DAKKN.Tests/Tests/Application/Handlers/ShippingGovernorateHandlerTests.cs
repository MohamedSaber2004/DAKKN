using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.ShippingGovernorates.Commands.CreateShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Commands.DeleteShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Commands.ToggleShippingGovernorateStatus;
using DAKKN.Application.Features.ShippingGovernorates.Commands.UpdateShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetShippingGovernorates;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class ShippingGovernorateHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;

        public ShippingGovernorateHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateShippingGovernorate_ShouldReturnDto()
        {
            var handler = new CreateShippingGovernorateCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(new CreateShippingGovernorateCommand("Cairo", "القاهرة", 50m, 1), default);

            result.Should().NotBeNull();
            result.Name.Should().Be("Cairo");
            result.ShippingPrice.Should().Be(50m);
        }

        [Fact]
        public async Task CreateShippingGovernorate_DuplicateName_ShouldThrow()
        {
            _context.ShippingGovernorates.Add(new ShippingGovernorate { Name = "Cairo", ArName = "القاهرة", IsActive = true });
            await _context.SaveChangesAsync();

            var handler = new CreateShippingGovernorateCommandHandler(_unitOfWork, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new CreateShippingGovernorateCommand("Cairo", "القاهرة", 50m, 1), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task UpdateShippingGovernorate_ShouldReturnDto()
        {
            var gov = new ShippingGovernorate { Name = "Old", ArName = "قديم", ShippingPrice = 10m, IsActive = true };
            _context.ShippingGovernorates.Add(gov);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new UpdateShippingGovernorateCommandHandler(_unitOfWork, _localizerMock.Object);
            var result = await handler.Handle(new UpdateShippingGovernorateCommand(gov.Id, "New", "جديد", 20m, 2, true), default);

            result.Name.Should().Be("New");
            result.ShippingPrice.Should().Be(20m);
        }

        [Fact]
        public async Task UpdateShippingGovernorate_NotFound_ShouldThrow()
        {
            var handler = new UpdateShippingGovernorateCommandHandler(_unitOfWork, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new UpdateShippingGovernorateCommand(Guid.NewGuid(), "N", "ج", 10m, 1, true), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteShippingGovernorate_ShouldSoftDelete()
        {
            var gov = new ShippingGovernorate { Name = "Del", ArName = "حذف", IsActive = true };
            _context.ShippingGovernorates.Add(gov);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new DeleteShippingGovernorateCommandHandler(_unitOfWork, _currentUserMock.Object);
            await handler.Handle(new DeleteShippingGovernorateCommand(gov.Id), default);

            var deleted = _context.ShippingGovernorates.IgnoreQueryFilters().FirstOrDefault(g => g.Id == gov.Id);
            deleted.Should().NotBeNull();
            deleted!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteShippingGovernorate_NotFound_ShouldThrow()
        {
            var handler = new DeleteShippingGovernorateCommandHandler(_unitOfWork, _currentUserMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteShippingGovernorateCommand(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task ToggleShippingGovernorateStatus_ShouldToggle()
        {
            var gov = new ShippingGovernorate { Name = "Tog", ArName = "تبديل", IsActive = true };
            _context.ShippingGovernorates.Add(gov);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ToggleShippingGovernorateStatusCommandHandler(_unitOfWork);
            await handler.Handle(new ToggleShippingGovernorateStatusCommand(gov.Id), default);

            var updated = _context.ShippingGovernorates.Find(gov.Id);
            updated!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task GetActiveShippingGovernorates_ShouldReturnActiveOnly()
        {
            _context.ShippingGovernorates.AddRange(
                new ShippingGovernorate { Name = "A", ArName = "أ", IsActive = true },
                new ShippingGovernorate { Name = "B", ArName = "ب", IsActive = false });
            await _context.SaveChangesAsync();

            var handler = new GetActiveShippingGovernoratesQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetActiveShippingGovernoratesQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetShippingGovernorates_WithSearch_ShouldFilter()
        {
            _context.ShippingGovernorates.AddRange(
                new ShippingGovernorate { Name = "Cairo", ArName = "القاهرة", IsActive = true },
                new ShippingGovernorate { Name = "Alex", ArName = "الإسكندرية", IsActive = true });
            await _context.SaveChangesAsync();

            var handler = new GetShippingGovernoratesQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetShippingGovernoratesQuery(SearchTerm: "Cairo"), default);

            result.Should().HaveCount(1);
        }
    }
}
