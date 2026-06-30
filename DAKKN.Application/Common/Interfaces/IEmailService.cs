namespace DAKKN.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken, CancellationToken ct = default);
        Task SendTicketCreatedEmailAsync(string toEmail, string fullName, string ticketNumber, string subject, CancellationToken ct = default);
        Task SendTicketReplyEmailAsync(string toEmail, string fullName, string ticketNumber, string replyMessage, bool isStaffReply, CancellationToken ct = default);
        Task SendTicketAssignedEmailAsync(string toEmail, string fullName, string ticketNumber, string adminName, CancellationToken ct = default);
        Task SendTicketClosedEmailAsync(string toEmail, string fullName, string ticketNumber, CancellationToken ct = default);
        Task SendNewTicketNotificationToAdminAsync(string adminEmail, string adminName, string ticketNumber, string customerName, string subject, CancellationToken ct = default);
    }
}
