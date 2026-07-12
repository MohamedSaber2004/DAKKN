using DAKKN.Application.Features.CustomOrders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrders
{
    public class GetCustomOrdersQueryHandler : IRequestHandler<GetCustomOrdersQuery, CustomOrderListResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomOrderListResultDto> Handle(GetCustomOrdersQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<CustomOrder>();
            var query = repo.GetAllAsync(null).AsNoTracking().Where(o => !o.IsDeleted);

            if (request.FilterStatus.HasValue)
                query = query.Where(o => o.Status == request.FilterStatus.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new CustomOrderListDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    ImageUrl = o.ImageUrl,
                    Quantity = o.Quantity,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            foreach (var order in orders)
            {
                if (!string.IsNullOrEmpty(order.ImageUrl))
                {
                    order.ImageUrl = string.Join(",", order.ImageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(f => f.StartsWith("http") || f.StartsWith("/") ? f : $"/files/{f}"));
                }
            }

            var baseQuery = repo.GetAllAsync(null).AsNoTracking().Where(o => !o.IsDeleted);
            var pendingCount = await baseQuery.CountAsync(o => o.Status == CustomOrderStatus.Pending, cancellationToken);
            var approvedCount = await baseQuery.CountAsync(o => o.Status == CustomOrderStatus.Approved, cancellationToken);
            var rejectedCount = await baseQuery.CountAsync(o => o.Status == CustomOrderStatus.Rejected, cancellationToken);

            return new CustomOrderListResultDto
            {
                Orders = orders,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount
            };
        }
    }
}
