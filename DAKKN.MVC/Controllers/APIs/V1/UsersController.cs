using Asp.Versioning;
using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
