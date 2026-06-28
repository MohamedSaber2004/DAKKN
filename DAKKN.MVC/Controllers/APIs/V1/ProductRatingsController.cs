using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.ProductRatings.Commands.RateProduct;
using DAKKN.Application.Features.ProductRatings.Queries.GetProductRatingSummary;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class ProductRatingsController : BaseApiController
    {
        public ProductRatingsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [Route(ApiRoutes.ProductRatings.GetSummary)]
        public async Task<IActionResult> GetSummary(Guid productId)
        {
            var result = await _mediator.Send(new GetProductRatingSummaryQuery { ProductId = productId });
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.ProductRatings.Rate)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Rate(Guid productId, [FromBody] RateProductCommand command)
        {
            command.ProductId = productId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
