using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CloseTicket
{
    public record CloseTicketCommand(Guid TicketId) : IRequest<bool>;
}
