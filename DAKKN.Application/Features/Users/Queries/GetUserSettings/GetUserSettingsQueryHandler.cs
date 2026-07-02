using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DAKKN.Application.Features.Users.Queries.GetUserSettings
{
    public class GetUserSettingsQueryHandler : IRequestHandler<GetUserSettingsQuery, UserSettingsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserSettingsQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<UserSettingsDto> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                throw new UnAuthorizedException();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), userId);

            var settingsRepo = _unitOfWork.GetRepository<UserSettings>();
            var settings = await settingsRepo.GetFirstAsync(x => x.UserId == userId);

            if (settings == null)
            {
                // Create default settings if not exists
                settings = new UserSettings
                {
                    UserId = userId,
                    Theme = "light",
                    IsDarkMode = false,
                    LayoutMode = "default",
                    CreatedBy = userId.ToString()
                };
                await settingsRepo.AddAsync(settings);
                await _unitOfWork.SaveChangesAsync();
            }

            return new UserSettingsDto
            {
                Language          = settings.Language,
                Theme             = settings.Theme,
                IsDarkMode        = settings.IsDarkMode,
                LayoutMode        = settings.LayoutMode,
                FullName          = user.FullName,
                Email             = user.Email ?? string.Empty,
                PhoneNumber       = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }
    }
}
