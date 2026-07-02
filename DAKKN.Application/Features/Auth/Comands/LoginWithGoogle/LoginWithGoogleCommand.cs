using DAKKN.Application.Features.Auth.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.LoginWithGoogle
{
    public sealed record LoginWithGoogleCommand(string IdToken, string? PhoneNumber = null, string? FullName = null) : IRequest<AuthResponseDto>;
}
