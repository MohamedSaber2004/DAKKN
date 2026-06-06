using DAKKN.Application.Common.Models;

namespace DAKKN.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<UploadResult> UploadAsync(Stream stream, string fileName, int place, string contentType, CancellationToken ct);
        Task<FileDeleteResult> DeleteAsync(string fileId, string fileName, CancellationToken ct);
        Task<string?> GetPublicUrlAsync(string fileName, int place, CancellationToken ct);
    }
}
