using Asp.Versioning;
using DAKKN.Appearence.Controllers.APIs;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion;
using DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetAllSuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById;
using DAKKN.Domain.Enums;
using DAKKN.Appearence.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class StickerSuggestionsController : BaseApiController
    {
        public StickerSuggestionsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [Route(ApiRoutes.StickerSuggestions.Base)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> Submit([FromForm] SubmitSuggestionCommand command)
        {
            var result = await _mediator.Send(command);
            return Created($"/api/v1/sticker-suggestions/{result.Id}", result);
        }

        [HttpGet]
        [Route(ApiRoutes.StickerSuggestions.My)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> GetMySuggestions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetMySuggestionsQuery(pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.StickerSuggestions.ById)]
        [RoleAuthorize(UserType.User, UserType.Admin)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSuggestionByIdQuery(id));
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.StickerSuggestions.Base)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] SuggestionStatus? status = null)
        {
            var result = await _mediator.Send(new GetAllSuggestionsQuery(pageNumber, pageSize, status));
            return Ok(result);
        }

        [HttpPatch]
        [Route(ApiRoutes.StickerSuggestions.Status)]
        [RoleAuthorize(UserType.Admin)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSuggestionStatusCommand command)
        {
            command.SuggestionId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
