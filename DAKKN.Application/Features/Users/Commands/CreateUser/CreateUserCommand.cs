using MediatR;
using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber,
        string Role,
        DateTime? BirthDate = null,
        Gender Gender = Gender.Male
    ) : IRequest<Guid>;
}
