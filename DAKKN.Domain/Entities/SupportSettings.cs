using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportSettings : BaseEntity<Guid>
    {
        public string SupportEmail { get; set; } = string.Empty;
        public string DefaultPriority { get; set; } = "Medium";
        public string DefaultResponseTime { get; set; } = "24h";
        public int MaxAttachmentSize { get; set; } = 10;
        public string AllowedExtensions { get; set; } = ".jpg,.jpeg,.png,.pdf,.docx,.zip";
        public int AutoCloseDays { get; set; } = 7;
        public bool NotifyOnNewTicket { get; set; } = true;
        public bool NotifyOnReply { get; set; } = true;
        public bool NotifyOnAssignment { get; set; } = true;
        public bool NotifyOnStatusChange { get; set; } = true;

        public static SupportSettings CreateDefault()
        {
            return new SupportSettings
            {
                Id = Guid.NewGuid(),
                SupportEmail = "support@dakkn.com",
                DefaultPriority = "Medium",
                DefaultResponseTime = "24h",
                MaxAttachmentSize = 10,
                AllowedExtensions = ".jpg,.jpeg,.png,.pdf,.docx,.zip",
                AutoCloseDays = 7,
                NotifyOnNewTicket = true,
                NotifyOnReply = true,
                NotifyOnAssignment = true,
                NotifyOnStatusChange = true,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
