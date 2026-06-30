using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportAttachment : BaseEntity<Guid>
    {
        public Guid TicketId { get; set; }
        public Guid? ReplyId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;

        public SupportTicket Ticket { get; set; } = null!;
        public SupportReply? Reply { get; set; }

        public static SupportAttachment Create(Guid ticketId, string fileName, string originalFileName,
            string contentType, long fileSize, string filePath, Guid? replyId = null)
        {
            return new SupportAttachment
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                ReplyId = replyId,
                FileName = fileName,
                OriginalFileName = originalFileName,
                ContentType = contentType,
                FileSize = fileSize,
                FilePath = filePath,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
