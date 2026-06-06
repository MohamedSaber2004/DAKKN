using Asp.Versioning;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Attachments.Commands.UploadAttachment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.Attachments.Base)]
    public class AttachmentController(IMediator mediator) : BaseApiController
    {
        [HttpPost(ApiRoutes.Attachments.Upload)]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] int place = 0)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<UploadResult>.Error("File is empty"));

            using var stream = file.OpenReadStream();
            var command = new UploadAttachmentCommand
            {
                Stream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Place = place
            };

            var result = await mediator.Send(command);

            if (result.Succeeded)
                return Ok(ApiResponse<UploadResult>.Ok(result, "File uploaded successfully"));

            return BadRequest(ApiResponse<UploadResult>.Error(result.Error ?? "Upload failed"));
        }
    }
}
