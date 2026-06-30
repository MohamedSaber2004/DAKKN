using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetTicketDetails
{
    public record GetTicketDetailsQuery(Guid TicketId, bool IsAdmin = false) : IRequest<SupportTicketDetailsDto>;
}
