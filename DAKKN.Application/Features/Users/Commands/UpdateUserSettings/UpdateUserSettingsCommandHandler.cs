using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Services;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Users.Commands.UpdateUserSettings
{
    public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, UserSettingsDto>
    {
        // Upload place 3 = Users folder (defined in UploadPaths / appsettings.json)
        private const int UsersUploadPlace = 3;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImageValidator _imageValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserSettingsCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager,
            IImageValidator imageValidator,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _imageValidator = imageValidator;
            _localizer = localizer;
        }

        public async Task<UserSettingsDto> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
                throw new UnAuthorizedException();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), userId);

            // ── Full-name update ─────────────────────────────────────────────────
            if (!string.IsNullOrEmpty(request.FullName))
            {
                user.UpdateFullName(request.FullName);
            }

            // ── Profile image: remove ────────────────────────────────────────────
            if (request.RemoveProfileImage && !string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                await _imageValidator.DeleteImage(user.ProfilePictureUrl, UsersUploadPlace);
                user.UpdateProfile(user.FullName, user.BirthDate, user.Gender, null);
            }

            // ── Profile image: upload new ────────────────────────────────────────
            if (request.ProfileImage != null && !request.RemoveProfileImage)
            {
                // Delete old file if one already exists
                if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
                    await _imageValidator.DeleteImage(user.ProfilePictureUrl, UsersUploadPlace);

                var upload = await _imageValidator.UploadImage(request.ProfileImage, UsersUploadPlace);
                if (!upload.Uploaded)
                    throw new BadRequestException(upload.Result);

                // Store only the filename – never a full URL
                user.UpdateProfile(user.FullName, user.BirthDate, user.Gender, upload.Result);
            }

            // Persist user-entity changes through Identity
            await _userManager.UpdateAsync(user);

            // ── User settings (theme / language / layout) ────────────────────────
            var settingsRepo = _unitOfWork.GetRepository<UserSettings>();
            var settings = await settingsRepo.GetFirstAsync(x => x.UserId == userId);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    UserId = userId,
                    CreatedBy = userId.ToString()
                };
                await settingsRepo.AddAsync(settings);
            }

            if (request.Language != null)   settings.Language  = request.Language;
            if (request.Theme != null)       settings.Theme      = request.Theme;
            if (request.IsDarkMode.HasValue) settings.IsDarkMode = request.IsDarkMode.Value;
            if (request.LayoutMode != null)  settings.LayoutMode = request.LayoutMode;

            settings.UpdatedAt = DateTime.UtcNow;
            settings.UpdatedBy = userId.ToString();

            await _unitOfWork.SaveChangesAsync();

            return new UserSettingsDto
            {
                Language        = settings.Language,
                Theme           = settings.Theme,
                IsDarkMode      = settings.IsDarkMode,
                LayoutMode      = settings.LayoutMode,
                FullName        = user.FullName,
                Email           = user.Email ?? string.Empty,
                PhoneNumber     = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }
    }
}
