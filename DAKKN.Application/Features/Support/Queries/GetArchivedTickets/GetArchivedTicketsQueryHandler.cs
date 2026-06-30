using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetArchivedTickets
{
    public class GetArchivedTicketsQueryHandler : IRequestHandler<GetArchivedTicketsQuery, PagginatedResult<SupportTicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetArchivedTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagginatedResult<SupportTicketDto>> Handle(GetArchivedTicketsQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportTicket>()
                .GetAllAsync(t => t.IsDeleted)
                .Include(t => t.Category)
                .OrderByDescending(t => t.DeletedAt)
                .Select(t => new SupportTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Subject = t.Subject,
                    CategoryName = t.Category.Name,
                    CategoryArName = t.Category.ArName,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    CustomerName = t.CustomerName ?? "",
                    CustomerEmail = t.CustomerEmail ?? "",
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });

            return await query.AsPagginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
