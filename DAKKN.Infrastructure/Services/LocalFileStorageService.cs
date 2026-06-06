using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Common.Services;
using Microsoft.Extensions.Hosting;

namespace DAKKN.Infrastructure.Services
{
    public class LocalFileStorageService(IHostEnvironment env) : IFileStorageService
    {
        private readonly string _webRootPath = Path.Combine(env.ContentRootPath, "wwwroot");

        public async Task<UploadResult> UploadAsync(Stream stream, string fileName, int place, string contentType, CancellationToken ct)
        {
            try
            {
                var folderPath = UploadPaths.GetPath(place);
                if (string.IsNullOrEmpty(folderPath))
                    return new UploadResult { Succeeded = false, Error = "Invalid upload place" };

                var directoryPath = Path.Combine(_webRootPath, folderPath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(directoryPath, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream, ct);
                }

                var publicUrl = await GetPublicUrlAsync(fileName, place, ct);

                return new UploadResult
                {
                    Succeeded = true,
                    FileName = fileName,
                    PublicUrl = publicUrl
                };
            }
            catch (Exception ex)
            {
                return new UploadResult { Succeeded = false, Error = ex.Message };
            }
        }

        public Task<FileDeleteResult> DeleteAsync(string fileId, string fileName, CancellationToken ct)
        {            
            try
            {
                foreach (var path in UploadPaths.GetAllPaths())
                {
                    if (string.IsNullOrWhiteSpace(path)) continue;
                    var filePath = Path.Combine(_webRootPath, path, fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return Task.FromResult(new FileDeleteResult { Succeeded = true });
                    }
                }
                
                return Task.FromResult(new FileDeleteResult { Succeeded = false, Error = "File not found" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new FileDeleteResult { Succeeded = false, Error = ex.Message });
            }
        }

        public Task<string?> GetPublicUrlAsync(string fileName, int place, CancellationToken ct)
        {
            // Matching CustomFileProvider convention: /files/{place}_{fileName}
            return Task.FromResult<string?>($"/files/{place}_{fileName}");
        }
    }
}
