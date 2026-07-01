using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Cart.Commands.AddToCart;
using DAKKN.Application.Features.Cart.Commands.RemoveFromCart;
using DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity;
using DAKKN.Application.Features.Cart.Commands.UpdateCartShipping;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Cart.Queries.GetCartCount;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using Microsoft.Extensions.Localization;
using System.Linq.Expressions;

namespace DAKKN.Tests.Tests.Application.Handlers
{
    public class CartHandlerTests
    {
        private readonly Mock<IGuestCartStorage> _cartStorageMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly Mock<IGenericRepository<Product>> _productRepoMock;
        private readonly Mock<IGenericRepository<ShippingGovernorate>> _govRepoMock;

        public CartHandlerTests()
        {
            _cartStorageMock = new Mock<IGuestCartStorage>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, key));
            _productRepoMock = new Mock<IGenericRepository<Product>>();
            _govRepoMock = new Mock<IGenericRepository<ShippingGovernorate>>();
            _unitOfWorkMock.Setup(u => u.GetRepository<Product>()).Returns(_productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.GetRepository<ShippingGovernorate>()).Returns(_govRepoMock.Object);
        }

        [Fact]
        public async Task AddToCart_NewItem_ShouldAdd()
        {
            var prod = new Product { Id = Guid.NewGuid(), Name = "P", ArName = "م", Price = 100m, QuantityInStock = 10, IsActive = true };
            _productRepoMock.Setup(r => r.FindByKeyAsync(prod.Id, default)).ReturnsAsync(prod);
            _cartStorageMock.Setup(x => x.GetCart()).Returns(new List<CartItemDto>());
            _cartStorageMock.Setup(x => x.GetCartCount()).Returns(1);

            var handler = new AddToCartCommandHandler(_unitOfWorkMock.Object, _cartStorageMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new AddToCartCommand(prod.Id, 2), default);

            result.Should().Be(1);
            _cartStorageMock.Verify(x => x.SetCart(It.Is<List<CartItemDto>>(l => l.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task AddToCart_ProductNotFound_ShouldThrow()
        {
            _productRepoMock.Setup(r => r.FindByKeyAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Product)null!);
            var handler = new AddToCartCommandHandler(_unitOfWorkMock.Object, _cartStorageMock.Object, _localizerMock.Object);

            await FluentActions.Invoking(() => handler.Handle(new AddToCartCommand(Guid.NewGuid(), 1), default))
                .Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddToCart_ExceedsStock_ShouldThrow()
        {
            var prod = new Product { Id = Guid.NewGuid(), Name = "P", ArName = "م", Price = 100m, QuantityInStock = 1, IsActive = true };
            _productRepoMock.Setup(r => r.FindByKeyAsync(prod.Id, default)).ReturnsAsync(prod);
            _cartStorageMock.Setup(x => x.GetCart()).Returns(new List<CartItemDto>());

            var handler = new AddToCartCommandHandler(_unitOfWorkMock.Object, _cartStorageMock.Object, _localizerMock.Object);
            await FluentActions.Invoking(() => handler.Handle(new AddToCartCommand(prod.Id, 5), default))
                .Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task RemoveFromCart_ShouldRemove()
        {
            var pid = Guid.NewGuid();
            _cartStorageMock.Setup(x => x.GetCart()).Returns(new List<CartItemDto> { new() { ProductId = pid, Quantity = 1 } });
            _cartStorageMock.Setup(x => x.GetCartCount()).Returns(0);

            var handler = new RemoveFromCartCommandHandler(_cartStorageMock.Object);
            var result = await handler.Handle(new RemoveFromCartCommand(pid), default);

            result.Should().Be(0);
        }

        [Fact]
        public async Task UpdateCartQuantity_ShouldUpdate()
        {
            var pid = Guid.NewGuid();
            var prod = new Product { Id = pid, Price = 100m, QuantityInStock = 10, IsActive = true };
            _productRepoMock.Setup(r => r.FindByKeyAsync(pid, default)).ReturnsAsync(prod);
            var cart = new List<CartItemDto> { new() { ProductId = pid, Quantity = 1 } };
            _cartStorageMock.Setup(x => x.GetCart()).Returns(cart);
            _cartStorageMock.Setup(x => x.GetCartCount()).Returns(() => cart.Sum(c => c.Quantity));

            var handler = new UpdateCartQuantityCommandHandler(_cartStorageMock.Object, _unitOfWorkMock.Object, _localizerMock.Object);
            var result = await handler.Handle(new UpdateCartQuantityCommand(pid, 3), default);

            result.Should().Be(3);
        }

        [Fact]
        public async Task GetCart_ShouldReturnItems()
        {
            var items = new List<CartItemDto> { new() { ProductId = Guid.NewGuid(), Name = "P", Price = 100m, Quantity = 2 } };
            _cartStorageMock.Setup(x => x.GetCart()).Returns(items);
            _cartStorageMock.Setup(x => x.GetShippingGovernorateId()).Returns((Guid?)null);

            var handler = new GetCartQueryHandler(_cartStorageMock.Object, _unitOfWorkMock.Object);
            var result = await handler.Handle(new GetCartQuery(), default);

            result.Items.Should().HaveCount(1);
            result.ShippingPrice.Should().Be(0);
        }

        [Fact]
        public async Task GetCartCount_ShouldReturnCount()
        {
            _cartStorageMock.Setup(x => x.GetCartCount()).Returns(3);
            var handler = new GetCartCountQueryHandler(_cartStorageMock.Object);
            var result = await handler.Handle(new GetCartCountQuery(), default);

            result.Should().Be(3);
        }
    }
}
