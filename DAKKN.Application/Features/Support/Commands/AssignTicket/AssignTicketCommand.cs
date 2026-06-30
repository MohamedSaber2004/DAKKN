using MediatR;

namespace DAKKN.Application.Features.Support.Commands.AssignTicket
{
    public record AssignTicketCommand(Guid TicketId, Guid AdminId, string AdminName) : IRequest<bool>;
}
