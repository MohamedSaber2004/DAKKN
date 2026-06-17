using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Users.Commands.ChangePassword;
using DAKKN.Application.Features.Users.Commands.CreateUser;
using DAKKN.Application.Features.Users.Commands.DeleteUser;
using DAKKN.Application.Features.Users.Commands.UpdateUser;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using DAKKN.Application.Features.Users.Queries.GetUserById;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    [RoleAuthorize(UserType.Admin)]
    public class UsersController : BaseApiController
    {
        public UsersController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Gets a paginated list of all users with optional filters.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paginated list of users.</returns>
        [HttpGet]
        [Route(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(PagginatedResult<UserListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        /// <summary>
        /// Gets a single user by ID.
        /// </summary>
        [HttpGet]
        [Route(ApiRoutes.Users.GetById)]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id), ct);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        [HttpPost]
        [Route(ApiRoutes.Users.Create)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Created(ApiRoutes.Users.GetById.Replace("{id}", result.ToString()), result);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        [HttpPut]
        [Route(ApiRoutes.Users.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest();
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        [HttpDelete]
        [Route(ApiRoutes.Users.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id), ct);
            return Ok(result);
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(ApiRoutes.Users.ChangePassword)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(id, request.Password), ct);
            return Ok(result);
        }

        public record ChangePasswordRequest(string Password);
    }
}
