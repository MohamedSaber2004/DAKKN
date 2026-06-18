using Asp.Versioning;
using DAKKN.Appearence.Controllers.APIs;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.MVC.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    [RoleAuthorize]
    public class SettingsController : BaseApiController
    {
        public SettingsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Get user settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(ApiRoutes.Settings.Get)]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType( StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetUserSettingsQuery());
            return Ok(result);
        }

        /// <summary>
        /// Update user settings
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(ApiRoutes.Settings.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateUserSettingsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
