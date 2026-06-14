using DAKKN.Application.Features.Auth.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.SignIn
{
    public record SignInCommand(string Email, string Password, bool RememberMe = false) : IRequest<AuthResponseDto>;
}
