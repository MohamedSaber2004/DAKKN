using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, AdminOrderListResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminOrderListResultDto> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var query = repo.GetAllAsync(null).AsNoTracking()
                .Include(o => o.Items)
                .Where(o => !o.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(o =>
                    o.OrderNumber.ToLower().Contains(term) ||
                    o.CustomerName.ToLower().Contains(term) ||
                    o.CustomerPhone.Contains(term) ||
                    (o.CustomerEmail != null && o.CustomerEmail.ToLower().Contains(term)) ||
                    o.TrackingNumber.ToLower().Contains(term));
            }

            if (request.FilterStatus.HasValue)
                query = query.Where(o => o.Status == request.FilterStatus.Value);

            if (request.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= request.ToDate.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderListDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    TrackingNumber = o.TrackingNumber,
                    CustomerName = o.CustomerName,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Subtotal = o.Subtotal,
                    ItemCount = o.Items.Count
                })
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var baseQuery = repo.GetAllAsync(null).AsNoTracking().Where(o => !o.IsDeleted);

            var pendingCount = await baseQuery.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);
            var processingCount = await baseQuery.CountAsync(o => o.Status == OrderStatus.Processing, cancellationToken);
            var shippedCount = await baseQuery.CountAsync(o => o.Status == OrderStatus.Shipped, cancellationToken);
            var deliveredCount = await baseQuery.CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken);
            var cancelledCount = await baseQuery.CountAsync(o => o.Status == OrderStatus.Cancelled, cancellationToken);
            var monthlyRevenue = await baseQuery
                .Where(o => o.CreatedAt >= startOfMonth && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Subtotal, cancellationToken);

            var monthlyDeliveryRevenue = await baseQuery
                .Where(o => o.CreatedAt >= startOfMonth && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.ShippingCost, cancellationToken);

            return new AdminOrderListResultDto
            {
                Orders = orders,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                PendingCount = pendingCount,
                ProcessingCount = processingCount,
                ShippedCount = shippedCount,
                DeliveredCount = deliveredCount,
                CancelledCount = cancelledCount,
                MonthlyRevenue = monthlyRevenue,
                MonthlyDeliveryRevenue = monthlyDeliveryRevenue
            };
        }
    }
}
