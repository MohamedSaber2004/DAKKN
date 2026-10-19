using DAKKN.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAKKN.Application.Features.Products.Commands.DeleteProductImage
{
    public class DeleteProductImagesCommandHandler : IRequestHandler<DeleteProductImagesCommand, bool>
    {
        private const int ProductsUploadPlace = 1;

        private static readonly string[] AllowedExtensions =
            { ".jpg", ".jpeg", ".webp", ".bmp", ".png", ".gif", ".jfif" };

        private readonly IImageValidator _imageValidator;
        private readonly ILogger<DeleteProductImagesCommandHandler> _logger;

        public DeleteProductImagesCommandHandler(IImageValidator imageValidator, ILogger<DeleteProductImagesCommandHandler> logger)
        {
            _imageValidator = imageValidator;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductImagesCommand request, CancellationToken cancellationToken)
        {
            if (request.FileNames == null || request.FileNames.Count == 0)
                return true;

            var allDeleted = true;

            foreach (var raw in request.FileNames)
            {
                if (!IsSafeFileName(raw))
                {
                    // Drop the request rather than erroring — the product row has already been
                    // committed, so failing here would strand the admin on a page that half-worked.
                    _logger.LogWarning(
                        "Rejected product image deletion with unsafe file name '{FileName}'.",
                        raw);
                    allDeleted = false;
                    continue;
                }

                // File already gone is not an error: the admin may have removed the same
                // image in a previous edit that also failed to delete.
                if (!await _imageValidator.DeleteImage(raw, ProductsUploadPlace))
                {
                    _logger.LogWarning(
                        "Product image '{FileName}' was not deleted from disk (missing or I/O error).",
                        raw);
                }
            }

            return allDeleted;
        }

        /// <summary>
        /// Rejects anything that is not a bare filename inside the uploads folder.
        /// <see cref="Common.Services.ImageValidator.DeleteImage"/> combines its argument with
        /// the uploads path without sanitizing, so an entry like "..\..\appsettings.json"
        /// from a tampered form field would otherwise delete an arbitrary file.
        /// </summary>
        private static bool IsSafeFileName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // No rooted paths, no directory separators.
            if (Path.IsPathRooted(name))
                return false;
            if (name.Contains('/') || name.Contains('\\') || name.Contains('\0'))
                return false;

            // Must resolve to exactly itself once normalized.
            if (!string.Equals(Path.GetFileName(name), name, StringComparison.Ordinal))
                return false;

            return AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant());
        }
    }
}
