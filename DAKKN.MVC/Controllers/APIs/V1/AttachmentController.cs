using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Attachments.Commands.UpdateImage;
using DAKKN.Application.Features.Attachments.Commands.UploadImage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    [RoleAuthorize]
    public class AttachmentController:BaseApiController
    {
        public AttachmentController(IMediator mediator): base(mediator) { }

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [Route(ApiRoutes.Attachments.UploadImage)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageCommand request)
        {
            var response = await _mediator.Send(request);
            return Created(ApiRoutes.Attachments.UploadImage, response);
        }

        /// <summary>
        /// Update Image
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPatch]
        [Route(ApiRoutes.Attachments.UpdateImage)]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateImage(string name, [FromForm] UpdateImageCommand request)
        {
            request.ImageName = name;
            var response = await _mediator.Send(request);
            return Accepted(response);
        }

        /// <summary>
        /// Upload List Of Images
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        [Route(ApiRoutes.Attachments.UploadMultiImage)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadListOfImages([FromForm] UploadMultipleImageCommand request)
        {
            var response = await _mediator.Send(request);
            return Created(ApiRoutes.Attachments.UploadMultiImage, response);
        }
    }
}
