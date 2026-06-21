using MediatR;

namespace DAKKN.Application.Features.AccountSecurity.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<Unit>;
}
