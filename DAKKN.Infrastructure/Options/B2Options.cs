namespace DAKKN.Infrastructure.Options
{
    public class B2Options
    {
        public string ApplicationKeyId { get; set; } = string.Empty;
        public string ApplicationKey { get; set; } = string.Empty;
        public string BucketId { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string PublicBaseUrl { get; set; } = string.Empty; // CDN or B2 friendly URL
    }
}
