using Asp.Versioning;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Auth.Comands.Logout;
using DAKKN.Application.Features.Auth.Comands.SignIn;
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


        /// <summary>
        /// Logins a user and issues tokens.
        /// </summary>
        /// <param name="command">Login credentials.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Authentication tokens and user info.</returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.Login)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(SignInCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        /// <summary>
        /// Logs out the user by revoking the refresh token.
        /// </summary>
        /// <param name="command">The logout details including refresh token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A success message.</returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.Logout)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
    }
}
