using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;

namespace DAKKN.Domain.Entities
{
    public class SupportTicket : BaseEntity<Guid>
    {
        public string TicketNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public SupportTicketPriority Priority { get; set; } = SupportTicketPriority.Medium;
        public SupportTicketStatus Status { get; set; } = SupportTicketStatus.Open;
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public string? Source { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public ApplicationUser? Customer { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
        public SupportCategory Category { get; set; } = null!;
        public ICollection<SupportReply> Replies { get; set; } = new List<SupportReply>();
        public ICollection<SupportAttachment> Attachments { get; set; } = new List<SupportAttachment>();
        public ICollection<SupportActivity> Activities { get; set; } = new List<SupportActivity>();
        public ICollection<SupportInternalNote> InternalNotes { get; set; } = new List<SupportInternalNote>();

        public static SupportTicket Create(Guid customerId, string customerName, string customerEmail,
            string subject, string message, Guid categoryId, SupportTicketPriority priority = SupportTicketPriority.Medium,
            string? customerPhone = null, string? source = null, string? orderNumber = null)
        {
            return new SupportTicket
            {
                Id = Guid.NewGuid(),
                TicketNumber = GenerateTicketNumber(),
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                Subject = subject,
                Message = message,
                CategoryId = categoryId,
                Priority = priority,
                Status = SupportTicketStatus.Open,
                Source = source,
                OrderNumber = orderNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        private static string GenerateTicketNumber()
        {
            return $"TK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}";
        }

        public void AssignTo(Guid adminId, string adminName)
        {
            AssignedToId = adminId;
            AssignedToName = adminName;
            Status = SupportTicketStatus.InProgress;
        }

        public void ReassignTo(Guid adminId, string adminName)
        {
            AssignedToId = adminId;
            AssignedToName = adminName;
        }

        public void UpdateStatus(SupportTicketStatus newStatus)
        {
            Status = newStatus;
            if (newStatus == SupportTicketStatus.Resolved)
                ResolvedAt = DateTime.UtcNow;
            else if (newStatus == SupportTicketStatus.Closed)
                ClosedAt = DateTime.UtcNow;
        }

        public void UpdatePriority(SupportTicketPriority newPriority)
        {
            Priority = newPriority;
        }

        public void RecordFirstResponse()
        {
            if (FirstResponseAt == null)
                FirstResponseAt = DateTime.UtcNow;
        }
    }
}
