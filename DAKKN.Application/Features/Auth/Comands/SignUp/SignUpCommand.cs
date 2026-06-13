using DAKKN.Application.Features.Auth.DTOs;
using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.SignUp
{
    public record SignupCommand(
        string FullName,
        string Email,
        string Password,
        string ConfirmPassword) : IRequest<AuthResponseDto>;
}
