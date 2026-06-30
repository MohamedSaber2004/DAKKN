using DAKKN.Application.Features.Support.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.Support.Commands.CreateTicket
{
    public record CreateTicketCommand : IRequest<CreateTicketResponseDto>
    {
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? OrderNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Source { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
