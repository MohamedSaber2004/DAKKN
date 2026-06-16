using Asp.Versioning;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Auth.Comands.ForgetPassword;
using DAKKN.Application.Features.Auth.Comands.LoginWithGoogle;
using DAKKN.Application.Features.Auth.Comands.Logout;
using DAKKN.Application.Features.Auth.Comands.SignIn;
using DAKKN.Application.Features.Auth.Comands.SignUp;
using DAKKN.Application.Features.Auth.Comands.VerifyForgetPasswordOtp;
using DAKKN.Application.Features.Auth.Commands.ResetPassword;
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

        /// <summary>
        /// Forgets the password for a user by sending a reset link or OTP to their email.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.ForgetPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        /// <summary>
        /// Verifies the OTP sent to the user's email for password reset.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.VerifyForgetPasswordOtp)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyForgetPasswordOtp(VerifyForgetPasswordOtpCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        /// <summary>
        /// Resets the user's password using the provided token and new password.
        /// </summary>
        /// <param name="command">The reset password details including token and new password.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A success message.</returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.ResetPassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);   
        }

        /// <summary>
        /// Logins a user using Google ID token.
        /// </summary>
        /// <param name="command">Google ID token.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Authentication tokens and user info.</returns>
        [HttpPost]
        [Route(ApiRoutes.Auth.LoginWithGoogle)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginWithGoogle(LoginWithGoogleCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
    }
}
