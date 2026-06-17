using MediatR;

namespace DAKKN.Application.Features.Users.Commands.ChangePassword
{
    public record ChangePasswordCommand(Guid Id, string Password) : IRequest<Unit>;
}
