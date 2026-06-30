using MediatR;

namespace DAKKN.Application.Features.Support.Commands.ArchiveTicket
{
    public record ArchiveTicketCommand(Guid TicketId) : IRequest<bool>;
}
