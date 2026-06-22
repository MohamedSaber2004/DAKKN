using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.BrandReviews.Commands.ApproveBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.CreateBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.RejectBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.ToggleDisplayBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.UpdateBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.UpdateDisplayOrderBrandReview;
using DAKKN.Application.Features.BrandReviews.Queries.GetAdminBrandReviews;
using DAKKN.Application.Features.BrandReviews.Queries.GetCustomerBrandReviews;
using DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class BrandReviewsController : BaseApiController
    {
        public BrandReviewsController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [Route(ApiRoutes.BrandReviews.Displayed)]
        public async Task<IActionResult> GetDisplayed()
        {
            var result = await _mediator.Send(new GetDisplayedBrandReviewsQuery());
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.BrandReviews.My)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new GetCustomerBrandReviewsQuery(userId));
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.BrandReviews.AdminAll)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetAdminReviews([FromQuery] GetAdminBrandReviewsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route(ApiRoutes.BrandReviews.Create)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateBrandReviewCommand command)
        {
            var userId = GetUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Created(ApiRoutes.BrandReviews.Create, result);
        }

        [HttpPut]
        [Route(ApiRoutes.BrandReviews.Update)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandReviewCommand command)
        {
            var userId = GetUserId();
            command = command with { Id = id, UserId = userId };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route(ApiRoutes.BrandReviews.Delete)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            await _mediator.Send(new DeleteBrandReviewCommand(id, userId));
            return Ok(true);
        }

        [HttpPost]
        [Route(ApiRoutes.BrandReviews.Approve)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Approve(Guid id)
        {
            var adminId = GetUserId();
            await _mediator.Send(new ApproveBrandReviewCommand(id, adminId));
            return Ok(true);
        }

        [HttpPost]
        [Route(ApiRoutes.BrandReviews.Reject)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _mediator.Send(new RejectBrandReviewCommand(id));
            return Ok(true);
        }

        [HttpPost]
        [Route(ApiRoutes.BrandReviews.ToggleDisplay)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> ToggleDisplay(Guid id)
        {
            await _mediator.Send(new ToggleDisplayBrandReviewCommand(id));
            return Ok(true);
        }

        [HttpPut]
        [Route(ApiRoutes.BrandReviews.UpdateDisplayOrder)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateDisplayOrder(Guid id, [FromBody] UpdateDisplayOrderBrandReviewCommand command)
        {
            command = command with { Id = id };
            await _mediator.Send(command);
            return Ok(true);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}
