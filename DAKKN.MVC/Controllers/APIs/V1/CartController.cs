using Asp.Versioning;
using DAKKN.Appearence.Controllers.APIs;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Cart.Commands.AddToCart;
using DAKKN.Application.Features.Cart.Commands.RemoveFromCart;
using DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity;
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Cart.Queries.GetCartCount;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class CartController : BaseApiController
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public CartController(IMediator mediator, IStringLocalizer<Messages> localizer) : base(mediator, localizer)
        {
            _localizer = localizer;
        }

        [HttpGet]
        [Route(ApiRoutes.Cart.Get)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCart()
        {
            var result = await _mediator.Send(new GetCartQuery());
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Cart.Count)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            var count = await _mediator.Send(new GetCartCountQuery());
            return Ok(count);
        }

        [HttpPost]
        [Route(ApiRoutes.Cart.Add)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
        {
            var count = await _mediator.Send(command);
            return Ok(count, _localizer[LocalizationKeys.CartMessages.Added.Key]);
        }

        [HttpDelete]
        [Route(ApiRoutes.Cart.Remove)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            var count = await _mediator.Send(new RemoveFromCartCommand(productId));
            return Ok(count, _localizer[LocalizationKeys.CartMessages.Removed.Key]);
        }

        [HttpPut]
        [Route(ApiRoutes.Cart.UpdateQuantity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartQuantityCommand command)
        {
            var count = await _mediator.Send(command);
            return Ok(count, _localizer[LocalizationKeys.CartMessages.Updated.Key]);
        }
    }
}
