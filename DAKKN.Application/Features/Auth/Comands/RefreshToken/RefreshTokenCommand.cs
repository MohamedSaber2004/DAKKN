using DAKKN.Application.Features.Auth.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Auth.Comands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponseDto>;
}
