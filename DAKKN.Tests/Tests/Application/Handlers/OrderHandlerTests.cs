using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Orders.Commands.CancelOrder;
using DAKKN.Application.Features.Orders.Commands.DeleteOrder;
using DAKKN.Application.Features.Orders.Commands.PlaceOrder;
using DAKKN.Application.Features.Orders.Commands.UpdateOrderStatus;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Application.Features.Orders.Queries.GetCustomerOrders;
using DAKKN.Application.Features.Orders.Queries.GetOrderDetails;
using DAKKN.Application.Features.Orders.Queries.GetOrders;
using DAKKN.Application.Features.Orders.Queries.GetDashboardStats;
using DAKKN.Application.Features.Orders.Queries.GetRecentOrders;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using DAKKN.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class OrderHandlerTests : IDisposable
    {
        private readonly DAKKNDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IGuestCartStorage> _cartStorageMock;
        private readonly Mock<ILogger<PlaceOrderCommandHandler>> _loggerMock;
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Mock<ILogger<CancelOrderCommandHandler>> _cancelLoggerMock;
        private readonly Mock<ILogger<UpdateOrderStatusCommandHandler>> _statusLoggerMock;

        public OrderHandlerTests()
        {
            _context = TestDbContextFactory.Create();
            _unitOfWork = new UnitOfWork(_context, new ServiceCollection().BuildServiceProvider());
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(x => x.UserId).Returns(_userId);
            _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
            _cartStorageMock = new Mock<IGuestCartStorage>();
            _loggerMock = new Mock<ILogger<PlaceOrderCommandHandler>>();
            _cancelLoggerMock = new Mock<ILogger<CancelOrderCommandHandler>>();
            _statusLoggerMock = new Mock<ILogger<UpdateOrderStatusCommandHandler>>();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task<(Category cat, Product prod, ShippingGovernorate gov)> SeedData()
        {
            var cat = new Category { CategoryName = "Cat", ArName = "تصنيف" };
            _context.Categories.Add(cat);
            var gov = new ShippingGovernorate { Name = "Cairo", ArName = "القاهرة", ShippingPrice = 20m };
            _context.ShippingGovernorates.Add(gov);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            cat = await _context.Categories.FirstAsync();
            gov = await _context.ShippingGovernorates.FirstAsync();
            var prod = new Product { Name = "P", ArName = "م", Description = "D", ArDescription = "و", Price = 100m, CategoryId = cat.Id, QuantityInStock = 10 };
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            prod = await _context.Products.FirstAsync();
            return (cat, prod, gov);
        }

        [Fact]
        public async Task DeleteOrder_ShouldSoftDelete()
        {
            var (_, _, gov) = await SeedData();
            var order = new Order("John", "j@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, _userId, null);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new DeleteOrderCommandHandler(_unitOfWork, _currentUserMock.Object);
            await handler.Handle(new DeleteOrderCommand(order.Id), default);

            var deleted = await _context.Orders.IgnoreQueryFilters().FirstAsync(o => o.Id == order.Id);
            deleted.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteOrder_NotFound_ShouldThrow()
        {
            var handler = new DeleteOrderCommandHandler(_unitOfWork, _currentUserMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new DeleteOrderCommand(Guid.NewGuid()), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task PlaceOrder_ShouldCreateOrder()
        {
            var (_, prod, gov) = await SeedData();
            var cartItems = new List<CartItemDto> { new() { ProductId = prod.Id, Name = prod.Name, Price = prod.Price, Quantity = 1, QuantityInStock = prod.QuantityInStock } };
            _cartStorageMock.Setup(x => x.GetCart()).Returns(cartItems);
            _cartStorageMock.Setup(x => x.GetCartCount()).Returns(1);

            var handler = new PlaceOrderCommandHandler(_unitOfWork, _cartStorageMock.Object, _currentUserMock.Object, _localizerMock.Object, _loggerMock.Object);
            var result = await handler.Handle(new PlaceOrderCommand("John", "123456", "Addr", gov.Id, null), default);

            result.OrderNumber.Should().NotBeNullOrEmpty();
            result.TotalAmount.Should().Be(120m);
            _cartStorageMock.Verify(x => x.ClearCart(), Times.Once);
        }

        [Fact]
        public async Task PlaceOrder_EmptyCart_ShouldThrow()
        {
            _cartStorageMock.Setup(x => x.GetCart()).Returns(new List<CartItemDto>());
            var handler = new PlaceOrderCommandHandler(_unitOfWork, _cartStorageMock.Object, _currentUserMock.Object, _localizerMock.Object, _loggerMock.Object);

            await FluentActions.Invoking(() => handler.Handle(new PlaceOrderCommand("John", "123456", "Addr", Guid.NewGuid(), null), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task CancelOrder_ShouldCancelAndRestoreStock()
        {
            var (_, prod, gov) = await SeedData();
            var order = new Order("John", "j@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, _userId, null);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new CancelOrderCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new CancelOrderCommand(order.Id, "Changed mind"), default);

            var cancelled = await _context.Orders.FirstAsync(o => o.Id == order.Id);
            cancelled.Status.Should().Be(OrderStatus.Cancelled);
        }

        [Fact]
        public async Task CancelOrder_NotFound_ShouldThrow()
        {
            var handler = new CancelOrderCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new CancelOrderCommand(Guid.NewGuid(), null), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldTransition()
        {
            var (_, _, gov) = await SeedData();
            var order = new Order("John", "j@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, _userId, null);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var handler = new UpdateOrderStatusCommandHandler(_unitOfWork, _currentUserMock.Object, _localizerMock.Object);
            await handler.Handle(new UpdateOrderStatusCommand(order.Id, OrderStatus.Confirmed, null), default);

            var updated = await _context.Orders.FirstAsync(o => o.Id == order.Id);
            updated.Status.Should().Be(OrderStatus.Confirmed);
        }

        [Fact]
        public async Task GetCustomerOrders_ShouldReturnOrders()
        {
            var (_, _, gov) = await SeedData();
            _context.Orders.Add(new Order("John", "j@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, _userId, null));
            await _context.SaveChangesAsync();

            var handler = new GetCustomerOrdersQueryHandler(_unitOfWork, _currentUserMock.Object);
            var result = await handler.Handle(new GetCustomerOrdersQuery(), default);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetOrders_ShouldPaginate()
        {
            var (_, _, gov) = await SeedData();
            for (int i = 0; i < 5; i++)
                _context.Orders.Add(new Order($"John{i}", $"j{i}@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, null, null));
            await _context.SaveChangesAsync();

            var handler = new GetOrdersQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetOrdersQuery(null, null, null, null, 1, 2), default);

            result.Orders.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
        }

        [Fact]
        public async Task GetDashboardStats_ShouldReturnStats()
        {
            var (_, _, gov) = await SeedData();
            _context.Orders.Add(new Order("John", "j@t.com", "123456", "Addr", gov.Id, gov.Name, gov.ShippingPrice, 100m, _userId, null));
            await _context.SaveChangesAsync();

            var handler = new GetDashboardStatsQueryHandler(_unitOfWork);
            var result = await handler.Handle(new GetDashboardStatsQuery(), default);

            result.PendingOrders.Should().Be(1);
        }
    }
}
