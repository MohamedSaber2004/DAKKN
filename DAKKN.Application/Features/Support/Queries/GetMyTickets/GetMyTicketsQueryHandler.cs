using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetMyTickets
{
    public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, PagginatedResult<SupportTicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMyTicketsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagginatedResult<SupportTicketDto>> Handle(GetMyTicketsQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportTicket>()
                .GetAllAsync(t => t.CustomerId == _currentUserService.UserId && !t.IsDeleted)
                .Include(t => t.Category)
                .Include(t => t.Replies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(t => t.Subject.Contains(request.Search) || t.TicketNumber.Contains(request.Search));

            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<Domain.Enums.SupportTicketStatus>(request.Status, true, out var status))
                query = query.Where(t => t.Status == status);

            query = query.OrderByDescending(t => t.CreatedAt);

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
