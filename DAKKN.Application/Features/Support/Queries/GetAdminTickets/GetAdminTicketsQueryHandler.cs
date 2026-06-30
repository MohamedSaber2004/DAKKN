using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetAdminTickets
{
    public class GetAdminTicketsQueryHandler : IRequestHandler<GetAdminTicketsQuery, PagginatedResult<SupportTicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagginatedResult<SupportTicketDto>> Handle(GetAdminTicketsQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportTicket>()
                .GetAllAsync(t => !t.IsDeleted)
                .Include(t => t.Category)
                .Include(t => t.Replies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(t => t.Subject.Contains(request.Search) || t.TicketNumber.Contains(request.Search)
                    || t.CustomerName.Contains(request.Search) || t.CustomerEmail.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<Domain.Enums.SupportTicketStatus>(request.Status, true, out var status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(request.Priority) && Enum.TryParse<Domain.Enums.SupportTicketPriority>(request.Priority, true, out var priority))
                query = query.Where(t => t.Priority == priority);

            if (request.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == request.CategoryId.Value);

            if (request.AssignedToId.HasValue)
                query = query.Where(t => t.AssignedToId == request.AssignedToId.Value);

            if (request.DateFrom.HasValue)
                query = query.Where(t => t.CreatedAt >= request.DateFrom.Value);

            if (request.DateTo.HasValue)
                query = query.Where(t => t.CreatedAt <= request.DateTo.Value);

            if (request.HasAttachments.HasValue && request.HasAttachments.Value)
                query = query.Where(t => t.Attachments.Any());

            query = (request.SortBy?.ToLower()) switch
            {
                "created" => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                "updated" => request.SortDescending ? query.OrderByDescending(t => t.UpdatedAt) : query.OrderBy(t => t.UpdatedAt),
                "priority" => request.SortDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                "status" => request.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            var projected = query.Select(t => new SupportTicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Subject = t.Subject,
                Message = t.Message,
                CategoryName = t.Category.Name,
                CategoryArName = t.Category.ArName,
                Priority = t.Priority.ToString(),
                Status = t.Status.ToString(),
                AssignedToName = t.AssignedToName ?? "",
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt,
                ReplyCount = t.Replies.Count
            });

            return await projected.AsPagginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
