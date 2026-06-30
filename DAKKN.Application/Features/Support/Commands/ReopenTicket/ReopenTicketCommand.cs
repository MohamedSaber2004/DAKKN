using MediatR;

namespace DAKKN.Application.Features.Support.Commands.ReopenTicket
{
    public record ReopenTicketCommand(Guid TicketId) : IRequest<bool>;
}
