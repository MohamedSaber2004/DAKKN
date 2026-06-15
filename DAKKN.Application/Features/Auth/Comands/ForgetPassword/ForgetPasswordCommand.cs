using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.ForgetPassword
{
    public record ForgetPasswordCommand(string Email) : IRequest<Unit>;
}
