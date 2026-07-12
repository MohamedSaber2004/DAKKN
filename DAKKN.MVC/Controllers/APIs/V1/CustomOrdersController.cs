using Asp.Versioning;
using DAKKN.Appearence.Controllers.APIs;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CustomOrders.Commands.ApproveCustomOrder;
using DAKKN.Application.Features.CustomOrders.Commands.CreateCustomOrder;
using DAKKN.Application.Features.CustomOrders.Commands.RejectCustomOrder;
using DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrderById;
using DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrders;
using DAKKN.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class CustomOrdersController : BaseApiController
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IImageValidator _imageValidator;

        public CustomOrdersController(IMediator mediator, IStringLocalizer<Messages> localizer, IImageValidator imageValidator) : base(mediator, localizer)
        {
            _localizer = localizer;
            _imageValidator = imageValidator;
        }

        [HttpPost]
        [Route(ApiRoutes.CustomOrders.Create)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCustomOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result, _localizer[LocalizationKeys.CustomOrderSubmission.Submitted.Value]);
        }

        [HttpPost]
        [Route(ApiRoutes.CustomOrders.Upload)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded");

            var result = await _imageValidator.UploadMultipleImage(files, 8);

            if (!result.Uploaded)
                return BadRequest(result.Result);

            var paths = result.Result.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            return Ok(paths);
        }
    }
}
