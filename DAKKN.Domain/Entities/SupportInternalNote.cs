using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportInternalNote : BaseEntity<Guid>
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public SupportTicket Ticket { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public static SupportInternalNote Create(Guid ticketId, Guid userId, string userName, string note)
        {
            return new SupportInternalNote
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UserId = userId,
                UserName = userName,
                Note = note,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}
