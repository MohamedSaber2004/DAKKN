using DAKKN.Application.Common.Models;
using MediatR;

namespace DAKKN.Application.Features.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery(
        string? FullName = null,
        string? Email = null,
        string? PhoneNumber = null,
        string? Role = null,  
        string? Status = null,      
        int PageNumber = 1,
        int PageSize = 10
        ) : IRequest<PagginatedResult<UserListItemDto>>;
}
