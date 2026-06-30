using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetMyTickets
{
    public record GetMyTicketsQuery : IRequest<PagginatedResult<SupportTicketDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; }
    }
}
