using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Products.Commands.DeleteProductImage
{
    /// <summary>
    /// Guards the one destructive call in this feature. <c>ImageValidator.DeleteImage</c> combines
    /// its argument with the uploads path without sanitizing it, so a name carrying a directory
    /// component would let the caller delete files outside the products folder. The handler has
    /// its own check as defence in depth; this rejects the bad batch before it is dispatched.
    /// </summary>
    public class DeleteProductImagesCommandValidator : AbstractValidator<DeleteProductImagesCommand>
    {
        private static readonly string[] AllowedExtensions =
            { ".jpg", ".jpeg", ".webp", ".bmp", ".png", ".gif", ".jfif" };

        public DeleteProductImagesCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.FileNames)
                .Must(files => files == null || files.Count <= ProductDto.MaxImages)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value,
                    ProductDto.MaxImages]);

            RuleForEach(v => v.FileNames)
                .Must(BeBareImageFileName)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidValue.Value]);
        }

        private static bool BeBareImageFileName(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            // Rooted paths, separators and traversal segments never reach a legitimate
            // product image, and each one widens what DeleteImage would touch.
            if (Path.IsPathRooted(fileName))
                return false;
            if (fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains('\0'))
                return false;
            if (!string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal))
                return false;

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return Array.IndexOf(AllowedExtensions, ext) >= 0;
        }
    }
}