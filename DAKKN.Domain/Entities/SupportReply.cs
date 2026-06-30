using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportReply : BaseEntity<Guid>
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsStaffReply { get; set; }

        public SupportTicket Ticket { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<SupportAttachment> Attachments { get; set; } = new List<SupportAttachment>();

        public static SupportReply Create(Guid ticketId, Guid userId, string userName, string message, bool isStaffReply)
        {
            return new SupportReply
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UserId = userId,
                UserName = userName,
                Message = message,
                IsStaffReply = isStaffReply,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
