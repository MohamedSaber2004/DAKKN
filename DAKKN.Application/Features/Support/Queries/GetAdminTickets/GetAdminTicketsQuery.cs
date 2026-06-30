using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetAdminTickets
{
    public record GetAdminTicketsQuery : IRequest<PagginatedResult<SupportTicketDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AssignedToId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
        public bool? HasAttachments { get; set; }
    }
}
