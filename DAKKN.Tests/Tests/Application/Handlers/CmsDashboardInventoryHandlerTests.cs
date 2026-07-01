using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CMS.Commands.UpdateLandingPageSettings;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.Application.Features.Dashboard.Queries.GetDashboardAnalytics;
using DAKKN.Application.Features.Dashboard.Queries.GetDashboardInventoryStats;
using DAKKN.Application.Features.Dashboard.Queries.GetRecentProductRatings;
using DAKKN.Application.Features.Inventory.Commands.ApplyGlobalDangerQuantity;
using DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings;
using DAKKN.Application.Features.Inventory.Queries.GetInventorySettings;
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
    public class CmsDashboardInventoryHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;

        public CmsDashboardInventoryHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetLandingPageSettings_ShouldReturnDefaults_WhenNoneExist()
        {
            var handler = new GetLandingPageSettingsQueryHandler(_context);
            var result = await handler.Handle(new GetLandingPageSettingsQuery(), default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateLandingPageSettings_ShouldUpdate()
        {
            var handler = new UpdateLandingPageSettingsCommandHandler(_context);
            var result = await handler.Handle(new UpdateLandingPageSettingsCommand
            {
                Hero = "{}",
                About = "{}"
            }, default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDashboardAnalytics_ShouldReturnData()
        {
            var handler = new GetDashboardAnalyticsQueryHandler(_context);
            var result = await handler.Handle(new GetDashboardAnalyticsQuery(7), default);

            result.Should().NotBeNull();
            result.DailyData.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDashboardInventoryStats_ShouldReturnStats()
        {
            var handler = new GetDashboardInventoryStatsQueryHandler(_context);
            var result = await handler.Handle(new GetDashboardInventoryStatsQuery(), default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetInventorySettings_ShouldReturnDefault_WhenNoneExist()
        {
            var handler = new GetInventorySettingsQueryHandler(_context);
            var result = await handler.Handle(new GetInventorySettingsQuery(), default);

            result.Should().NotBeNull();
            result.GlobalDangerQuantity.Should().Be(10);
        }

        [Fact]
        public async Task UpdateInventorySettings_ShouldUpdate()
        {
            var handler = new UpdateInventorySettingsCommandHandler(_context, _localizerMock.Object);
            var result = await handler.Handle(new UpdateInventorySettingsCommand(5), default);

            result.GlobalDangerQuantity.Should().Be(5);
        }

        [Fact]
        public async Task ApplyGlobalDangerQuantity_ShouldApply()
        {
            var cat = new Category { CategoryName = "C", ArName = "ج" };
            _context.Categories.Add(cat);
            _context.Products.Add(new Product { Name = "P", ArName = "م", Description = "D", ArDescription = "و", Price = 10m, CategoryId = cat.Id, QuantityInStock = 10, DangerQuantity = 0 });
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new ApplyGlobalDangerQuantityCommandHandler(_context);
            var updated = await handler.Handle(new ApplyGlobalDangerQuantityCommand(), default);

            updated.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetRecentProductRatings_ShouldReturnList()
        {
            var handler = new GetRecentProductRatingsQueryHandler(new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider()));
            var result = await handler.Handle(new GetRecentProductRatingsQuery(5), default);

            result.Should().NotBeNull();
        }
    }
}
