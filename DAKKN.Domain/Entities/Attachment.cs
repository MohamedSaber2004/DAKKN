using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class Attachment : BaseEntity<Guid>
    {
        public string FileName { get; private set; } = string.Empty;
        public string? B2FileId { get; private set; } 
        public int Place { get; private set; } 
        public long SizeBytes { get; private set; }
        public string ContentType { get; private set; } = string.Empty;

        private Attachment() { } 

        public Attachment(string fileName, string? b2FileId, int place, long sizeBytes, string contentType)
        {
            FileName = fileName;
            B2FileId = b2FileId;
            Place = place;
            SizeBytes = sizeBytes;
            ContentType = contentType;
        }
    }
}
