using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateTicketPriority
{
    public record UpdateTicketPriorityCommand(Guid TicketId, string NewPriority) : IRequest<bool>;
}
