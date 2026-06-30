using DAKKN.Application.Features.Support.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.Support.Commands.ReplyTicket
{
    public record ReplyTicketCommand : IRequest<SupportReplyDto>
    {
        public Guid TicketId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsStaffReply { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
