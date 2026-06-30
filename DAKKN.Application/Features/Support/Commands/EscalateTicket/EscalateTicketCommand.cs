using MediatR;

namespace DAKKN.Application.Features.Support.Commands.EscalateTicket
{
    public record EscalateTicketCommand(Guid TicketId, string Reason) : IRequest<bool>;
}
