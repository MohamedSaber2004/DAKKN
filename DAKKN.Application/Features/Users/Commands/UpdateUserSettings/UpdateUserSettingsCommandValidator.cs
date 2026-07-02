using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Users.Commands.UpdateUserSettings
{
    public class UpdateUserSettingsCommandValidator : AbstractValidator<UpdateUserSettingsCommand>
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserSettingsCommandValidator(IStringLocalizer<Messages> localizer, IImageValidator imageValidator)
        {
            _localizer = localizer;

            RuleFor(x => x.FullName)
                .MaximumLength(100)
                .WithMessage(_localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 100]);

            RuleFor(x => x.Language)
                .Must(x => x == null || x == "ar" || x == "en")
                .WithMessage(_localizer[LocalizationKeys.UserSettings.InvalidLanguage.Value]);

            RuleFor(x => x.Theme)
                .Must(x => x == null || x == "light" || x == "dark" || x == "system")
                .WithMessage(_localizer[LocalizationKeys.UserSettings.InvalidTheme.Value]);

            // Profile image validation (only when a file is submitted)
            RuleFor(x => x.ProfileImage)
                .Must(file => file == null || imageValidator.IsValidImage(file))
                .WithMessage(_localizer[LocalizationKeys.UploadFileMessages.FileNotValid.Value]);

            RuleFor(x => x.ProfileImage)
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage(_localizer[LocalizationKeys.ProfileImageMessages.FileTooLarge.Value]);
        }
    }
}
