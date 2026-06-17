using MediatR;
using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(
        Guid Id,
        string FullName,
        string Email,
        string? PhoneNumber,
        string Role,
        bool IsActive,
        string? Password = null,
        Gender? Gender = null,
        DateTime? BirthDate = null
    ) : IRequest<Unit>;
}
