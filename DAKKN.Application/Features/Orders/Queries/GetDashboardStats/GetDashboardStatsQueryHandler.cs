using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var userRepo = _unitOfWork.GetRepository<ApplicationUser>();

            var now = DateTime.UtcNow;
            var startOfToday = now.Date;
            var startOf24HoursAgo = now.AddHours(-24);

            var baseQuery = orderRepo.GetAllAsync(null).AsNoTracking().Where(o => !o.IsDeleted);

            var ordersToday = await baseQuery.CountAsync(o => o.CreatedAt >= startOfToday, cancellationToken);
            var ordersLast24 = await baseQuery.CountAsync(o => o.CreatedAt >= startOf24HoursAgo, cancellationToken);

            var revenueToday = await baseQuery
                .Where(o => o.CreatedAt >= startOfToday && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount, cancellationToken);

            var revenueLast24 = await baseQuery
                .Where(o => o.CreatedAt >= startOf24HoursAgo && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount, cancellationToken);

            var pendingOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken);
            var confirmedOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Confirmed, cancellationToken);
            var processingOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Processing, cancellationToken);
            var shippedOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Shipped, cancellationToken);
            var deliveredOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Delivered, cancellationToken);
            var cancelledOrders = await baseQuery.CountAsync(o => o.Status == OrderStatus.Cancelled, cancellationToken);

            var totalProducts = await productRepo.GetAllAsync(null).AsNoTracking()
                .CountAsync(p => !p.IsDeleted, cancellationToken);
            var totalUsers = await userRepo.GetAllAsync(null).AsNoTracking()
                .CountAsync(u => !u.IsDeleted, cancellationToken);

            return new DashboardStatsDto
            {
                OrdersToday = ordersToday,
                OrdersLast24Hours = ordersLast24,
                RevenueToday = revenueToday,
                RevenueLast24Hours = revenueLast24,
                PendingOrders = pendingOrders,
                ConfirmedOrders = confirmedOrders,
                ProcessingOrders = processingOrders,
                ShippedOrders = shippedOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers
            };
        }
    }
}
