using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Services;
using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace DAKKN.Application.Features.Attachments.Commands.UpdateImage
{
    public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IImageValidator _imageValidator;

        public UpdateImageCommandValidator(IStringLocalizer<Messages> localizer, IImageValidator imageValidator)
        {
            _localizer = localizer;
            _imageValidator = imageValidator;

            RuleFor(x => x.UploadPlace)
                .NotNull().WithMessage(x => _localizer[LocalizationKeys.UploadFileMessages.PlaceRequried.Value]);

            RuleFor(x => x.File)
                .NotNull().NotEmpty().WithMessage(x => _localizer[LocalizationKeys.UploadFileMessages.Requried.Value]);

            // If ImageName is provided, its first digit must equal UploadPlace
            RuleFor(x => x.ImageName)
                .Must((command, imageName) =>
                {
                    if (string.IsNullOrWhiteSpace(imageName))
                        return true;

                    // find the first single digit character
                    var match = Regex.Match(imageName, @"\d");
                    if (!match.Success)
                        return false;

                    if (!int.TryParse(match.Value, out var firstDigit))
                        return false;

                    return firstDigit == command.UploadPlace;
                })
                .WithMessage(x => _localizer[LocalizationKeys.UploadFileMessages.PlaceNotValid.Value]);

            RuleFor(x => x.ImageName)
                .Must((command, imageName) =>
                {
                    if (string.IsNullOrWhiteSpace(imageName))
                        return true;

                    return _imageValidator.IsValidImage(imageName, UploadPaths.GetPath(command.UploadPlace) ?? string.Empty);
                })
                .WithMessage(x => _localizer[LocalizationKeys.UploadFileMessages.FileNotFound.Value]);
        }
    }
}
