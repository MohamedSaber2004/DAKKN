using MediatR;

namespace DAKKN.Application.Features.AccountSecurity.Commands.DeleteAccount
{
    public record DeleteAccountCommand(
        string CurrentPassword,
        string ConfirmationText
    ) : IRequest<Unit>;
}
