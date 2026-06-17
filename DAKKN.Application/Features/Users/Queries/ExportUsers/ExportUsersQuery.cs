using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using MediatR;
using System.Collections.Generic;

namespace DAKKN.Application.Features.Users.Queries.ExportUsers
{
    public record ExportUsersQuery(
        string? SearchTerm = null,
        string? Role = null,
        string? Status = null
    ) : IRequest<List<UserListItemDto>>;
}
