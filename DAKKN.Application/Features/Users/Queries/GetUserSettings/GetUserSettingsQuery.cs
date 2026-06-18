using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Users.Queries.GetUserSettings
{
    public record GetUserSettingsQuery : IRequest<UserSettingsDto>;
}
