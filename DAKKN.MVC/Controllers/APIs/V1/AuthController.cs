using Asp.Versioning;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Auth.Comands.SignUp;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class AuthController:BaseApiController
    {
        public AuthController(IMediator mediator):base(mediator)
        {
            
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="command">Registration details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Authentication tokens and user info.</returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.Signup)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Signup(SignupCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Created(null!, result);
        }
    }
}
