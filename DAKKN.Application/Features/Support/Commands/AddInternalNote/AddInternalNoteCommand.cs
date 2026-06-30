using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.AddInternalNote
{
    public record AddInternalNoteCommand : IRequest<SupportInternalNoteDto>
    {
        public Guid TicketId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
