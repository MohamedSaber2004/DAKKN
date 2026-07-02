using DAKKN.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.Users.Commands.UpdateUserSettings
{
    public record UpdateUserSettingsCommand(
        string? FullName,
        string? Language,
        string? Theme,
        bool? IsDarkMode,
        string? LayoutMode,
        IFormFile? ProfileImage = null,
        bool RemoveProfileImage = false
    ) : IRequest<UserSettingsDto>;
}
