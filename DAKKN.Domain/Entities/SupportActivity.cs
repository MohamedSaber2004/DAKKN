using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportActivity : BaseEntity<Guid>
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        public SupportTicket Ticket { get; set; } = null!;

        public static SupportActivity Create(Guid ticketId, Guid userId, string userName, string action,
            string? details = null, string? oldValue = null, string? newValue = null)
        {
            return new SupportActivity
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UserId = userId,
                UserName = userName,
                Action = action,
                Details = details,
                OldValue = oldValue,
                NewValue = newValue,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
