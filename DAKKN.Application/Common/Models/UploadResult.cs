namespace DAKKN.Application.Common.Models
{
    public class UploadResult
    {
        public bool Succeeded { get; set; }
        public string? FileId { get; set; }
        public string? FileName { get; set; }
        public string? PublicUrl { get; set; }
        public string? Error { get; set; }
    }
}
