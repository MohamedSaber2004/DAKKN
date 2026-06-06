using B2Net;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Common.Services;
using DAKKN.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace DAKKN.Infrastructure.Services
{
    public class B2FileStorageService : IFileStorageService
    {
        private readonly B2Options _options;
        private readonly B2Client _client;

        public B2FileStorageService(IOptions<B2Options> options)
        {
            _options = options.Value;
            
            var b2Options = new B2Net.Models.B2Options
            {
                KeyId = _options.ApplicationKeyId,
                ApplicationKey = _options.ApplicationKey,
                BucketId = _options.BucketId,
                PersistBucket = true
            };

            _client = new B2Client(b2Options);
        }

        public async Task<UploadResult> UploadAsync(Stream stream, string fileName, int place, string contentType, CancellationToken ct)
        {
            try
            {
                var folderPath = UploadPaths.GetPath(place);
                if (string.IsNullOrEmpty(folderPath))
                    return new UploadResult { Succeeded = false, Error = "Invalid upload place" };

                // B2 path: {folderPath}/{fileName}
                var b2FileName = $"{folderPath}/{fileName}";

                byte[] fileData;
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms, ct);
                    fileData = ms.ToArray();
                }

                var file = await _client.Files.Upload(fileData, b2FileName, _options.BucketId);

                return new UploadResult
                {
                    Succeeded = true,
                    FileId = file.FileId,
                    FileName = fileName,
                    PublicUrl = $"{_options.PublicBaseUrl}/{_options.BucketName}/{b2FileName}"
                };
            }
            catch (Exception ex)
            {
                return new UploadResult { Succeeded = false, Error = ex.Message };
            }
        }

        public async Task<FileDeleteResult> DeleteAsync(string fileId, string fileName, CancellationToken ct)
        {
            try
            {
                // B2 requires fileId and fileName for deletion
                await _client.Files.Delete(fileId, fileName);
                return new FileDeleteResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                return new FileDeleteResult { Succeeded = false, Error = ex.Message };
            }
        }

        public Task<string?> GetPublicUrlAsync(string fileName, int place, CancellationToken ct)
        {
            var folderPath = UploadPaths.GetPath(place);
            if (string.IsNullOrEmpty(folderPath))
                return Task.FromResult<string?>(null);

            var b2FileName = $"{folderPath}/{fileName}";
            return Task.FromResult<string?>($"{_options.PublicBaseUrl}/{_options.BucketName}/{b2FileName}");
        }
    }
}
