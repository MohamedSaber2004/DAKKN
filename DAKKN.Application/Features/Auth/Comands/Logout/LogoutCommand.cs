using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<string>;
}
