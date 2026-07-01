using DAKKN.Appearence.Controllers.APIs.V1;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Cart.Commands.AddToCart;
using DAKKN.Application.Features.Cart.Commands.RemoveFromCart;
using DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity;
using DAKKN.Application.Features.Cart.Commands.UpdateCartShipping;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Cart.Queries.GetCartCount;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Controllers
{
    public class CartControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly CartController _controller;
        private static readonly string LocalizedStringValue = "localized";

        public CartControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns(new LocalizedString("key", LocalizedStringValue));

            _controller = new CartController(_mediatorMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetCart_ShouldReturnOk()
        {
            var cartDto = new CartDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cartDto);

            var result = await _controller.GetCart();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<CartDto>>().Subject;
            apiResponse.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetCount_ShouldReturnOkWithCount()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartCountQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _controller.GetCount();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<int>>().Subject;
            apiResponse.Data.Should().Be(5);
        }

        [Fact]
        public async Task AddToCart_ShouldReturnOkWithLocalizedMessage()
        {
            var command = new AddToCartCommand(Guid.NewGuid(), 2);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var result = await _controller.AddToCart(command);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<int>>().Subject;
            apiResponse.Message.Should().Be(LocalizedStringValue);
            apiResponse.Data.Should().Be(3);
        }

        [Fact]
        public async Task RemoveFromCart_ShouldReturnOk()
        {
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveFromCartCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _controller.RemoveFromCart(productId);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<int>>().Subject;
            apiResponse.Data.Should().Be(2);
        }

        [Fact]
        public async Task UpdateQuantity_ShouldReturnOk()
        {
            var command = new UpdateCartQuantityCommand(Guid.NewGuid(), 5);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);

            var result = await _controller.UpdateQuantity(command);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateShipping_ShouldReturnOk()
        {
            var command = new UpdateCartShippingCommand(Guid.NewGuid());
            var shippingResult = new CartDto();
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(shippingResult);

            var result = await _controller.UpdateShipping(command);

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
