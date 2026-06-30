using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateTicketStatus
{
    public record UpdateTicketStatusCommand(Guid TicketId, string NewStatus) : IRequest<bool>;
}
