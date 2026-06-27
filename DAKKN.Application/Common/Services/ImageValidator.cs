using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Localization;
using System.Text;

namespace DAKKN.Application.Common.Services
{
    public class ImageValidator : IImageValidator
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileExtensionContentTypeProvider _contentProvider;
        private readonly IStringLocalizer<Messages> _localizer;

        public ImageValidator(IWebHostEnvironment env, FileExtensionContentTypeProvider contentProvider, IStringLocalizer<Messages> localizer)
        {
            _env = env;
            _contentProvider = contentProvider;
            _localizer = localizer;
        }

        public async Task<(bool Uploaded, string Result)> UploadImage(IFormFile file, int place = 0)
        {
            try
            {
                if (file == null || file.Length <= 0)
                    return (false, _localizer[LocalizationKeys.UploadFileMessages.FileNotFound.Value]);

                if (string.IsNullOrWhiteSpace(file.FileName))
                    return (false, _localizer[LocalizationKeys.UploadFileMessages.FileNotValid.Value]);

                var extension = Path.GetExtension(file.FileName)?.ToLower();

                // Handle null or empty ContentType
                if (string.IsNullOrWhiteSpace(file.ContentType))
                {
                    // For GIF files, we'll validate them later with IsValidImage and IsValidGifFile
                    if (extension != ".gif")
                    {
                        return (false, _localizer[LocalizationKeys.UploadFileMessages.InvalidContentType.Value]);
                    }
                    // Allow GIF to proceed even without ContentType
                }

                // Validate the image
                if (!IsValidImage(file))
                    return (false, _localizer[LocalizationKeys.UploadFileMessages.FileNotValid.Value]);

                var x = UploadPaths.GetPath(place);
                string uploadPlace = Path.Combine(_env.WebRootPath ?? string.Empty, UploadPaths.GetPath(place) ?? string.Empty);

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadPlace))
                {
                    Directory.CreateDirectory(uploadPlace);
                }

                var uniqueFileName = $"{place}{GetUniqueFileName(file.FileName)}";
                var filePath = Path.Combine(uploadPlace, uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Verify file was created
                if (!File.Exists(filePath))
                    return (false, _localizer[LocalizationKeys.UploadFileMessages.FileUploadFailed.Value]);

                // Additional GIF validation after upload
                if (extension == ".gif")
                {
                    if (!await IsValidGifFile(filePath))
                    {
                        File.Delete(filePath);
                        return (false, _localizer[LocalizationKeys.UploadFileMessages.FileNotValid.Value]);
                    }
                }

                return (true, uniqueFileName);
            }
            catch
            {
                return (false, _localizer[LocalizationKeys.UploadFileMessages.FileUploadFailed.Value]);
            }
        }

        private async Task<bool> IsValidGifFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    return false;

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (fileStream.Length < 6)
                        return false;

                    byte[] header = new byte[6];
                    int bytesRead = await fileStream.ReadAsync(header, 0, 6);

                    if (bytesRead < 6)
                        return false;

                    string signature = Encoding.ASCII.GetString(header);
                    return signature == "GIF89a" || signature == "GIF87a";
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteImage(string fileName, int place)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return false;

                string uploadPlace = Path.Combine(_env.WebRootPath ?? string.Empty, UploadPaths.GetPath(place) ?? string.Empty);
                string filePath = Path.Combine(uploadPlace, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetUniqueFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return $"{DateTime.Now:yyyyMMddHHmmssfff}.jpg";

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";

            return $"{timestamp}{extension}";
        }

        public bool IsValidImage(IFormFile file)
        {
            const int maxImageFileSize = 10 * 1024 * 1024;

            if (file == null || file.Length == 0 || file.Length > maxImageFileSize)
                return false;

            if (string.IsNullOrWhiteSpace(file.FileName))
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".webp", ".bmp", ".png", ".gif", ".jfif" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLower();

            if (string.IsNullOrWhiteSpace(fileExtension) || !allowedExtensions.Contains(fileExtension))
                return false;

            // If content type is provided, validate it
            var contentType = file.ContentType?.ToLower();
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                var allowedContentTypes = new[]
                {
                    "image/jpeg", "image/jpg", "image/png", "image/gif",
                    "image/webp", "image/bmp", "image/x-ms-bmp", "image/jfif",
                    "image/pjpeg", "image/x-gif", "application/octet-stream"
                };

                if (!allowedContentTypes.Contains(contentType))
                    return false;
            }

            return true;
        }

        public bool IsValidImage(string ImageName, string PlaceHolder)
        {
            if (string.IsNullOrWhiteSpace(ImageName) || string.IsNullOrWhiteSpace(PlaceHolder))
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".webp", ".bmp", ".png", ".gif", ".jfif" };

            var Result = allowedExtensions.Any(x => ImageName.ToLower().EndsWith(x));
            if (Result)
            {
                string uploadPlace = Path.Combine(_env.WebRootPath ?? string.Empty, PlaceHolder);
                string? fullImagePath = Path.Combine(uploadPlace, ImageName);
                return ImageIsExisted(fullImagePath);
            }
            return false;
        }

        public bool ImageIsExisted(string? fullImagePath)
        {
            if (string.IsNullOrWhiteSpace(fullImagePath))
                return false;

            var image = Path.Combine(Directory.GetCurrentDirectory(), fullImagePath);
            return File.Exists(image);
        }

        public async Task<(bool Uploaded, string Result)> UploadMultipleImage(List<IFormFile> file, int Place)
        {
            if (file == null || !file.Any())
                return (false, _localizer[LocalizationKeys.UploadFileMessages.Requried.Value]);

            StringBuilder stringResult = new StringBuilder();

            foreach (var item in file)
            {
                if (item == null)
                    continue;

                var result = await UploadImage(item, Place);
                if (!result.Uploaded)
                    return (false, result.Result);

                stringResult.Append(result.Result + ";");
            }

            return (true, stringResult.ToString());
        }

        public async Task<IFormFile> ConvertImageToFormFile(string imageUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(imageUrl);
                var imageStream = await response.Content.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileName = Path.GetFileName(new Uri(imageUrl).AbsolutePath);
                var extension = Path.GetExtension(fileName)?.ToLower();

                // If no extension, determine from Content-Type header
                if (string.IsNullOrWhiteSpace(extension))
                {
                    var contentTypeHeader = response.Content.Headers.ContentType?.MediaType?.ToLower();
                    extension = contentTypeHeader switch
                    {
                        "image/jpeg" or "image/jpg" => ".jpg",
                        "image/png" => ".png",
                        "image/gif" => ".gif",
                        "image/webp" => ".webp",
                        "image/bmp" or "image/x-ms-bmp" => ".bmp",
                        "image/jfif" => ".jfif",
                        _ => ".jpg"
                    };

                    fileName = $"{Path.GetFileNameWithoutExtension(fileName)}{extension}";
                }

                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    ".jfif" => "image/jfif",
                    _ => "application/octet-stream"
                };

                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };

                return formFile;
            }
        }
    }
}
