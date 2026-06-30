using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetAdminTicketDetails
{
    public record GetAdminTicketDetailsQuery(Guid TicketId) : IRequest<SupportTicketDetailsDto>;
}
