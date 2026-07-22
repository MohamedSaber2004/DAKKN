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

            var orderStats = await baseQuery
                .GroupBy(o => 1)
                .Select(g => new
                {
                    OrdersToday = g.Count(o => o.CreatedAt >= startOfToday),
                    OrdersLast24 = g.Count(o => o.CreatedAt >= startOf24HoursAgo),
                    RevenueToday = g.Where(o => o.CreatedAt >= startOfToday && o.Status == OrderStatus.Delivered).Sum(o => o.Subtotal),
                    RevenueLast24 = g.Where(o => o.CreatedAt >= startOf24HoursAgo && o.Status == OrderStatus.Delivered).Sum(o => o.Subtotal),
                    DeliveryRevenueToday = g.Where(o => o.CreatedAt >= startOfToday && o.Status == OrderStatus.Delivered).Sum(o => o.ShippingCost),
                    DeliveryRevenueLast24 = g.Where(o => o.CreatedAt >= startOf24HoursAgo && o.Status == OrderStatus.Delivered).Sum(o => o.ShippingCost),
                    PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                    ConfirmedOrders = g.Count(o => o.Status == OrderStatus.Confirmed),
                    ProcessingOrders = g.Count(o => o.Status == OrderStatus.Processing),
                    ShippedOrders = g.Count(o => o.Status == OrderStatus.Shipped),
                    DeliveredOrders = g.Count(o => o.Status == OrderStatus.Delivered),
                    CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled)
                })
                .FirstOrDefaultAsync(cancellationToken);

            var totalProducts = await productRepo.GetAllAsync(null).AsNoTracking()
                .CountAsync(p => !p.IsDeleted, cancellationToken);
            var totalUsers = await userRepo.GetAllAsync(null).AsNoTracking()
                .CountAsync(u => !u.IsDeleted, cancellationToken);

            return new DashboardStatsDto
            {
                OrdersToday = orderStats?.OrdersToday ?? 0,
                OrdersLast24Hours = orderStats?.OrdersLast24 ?? 0,
                RevenueToday = orderStats?.RevenueToday ?? 0,
                RevenueLast24Hours = orderStats?.RevenueLast24 ?? 0,
                DeliveryRevenueToday = orderStats?.DeliveryRevenueToday ?? 0,
                DeliveryRevenueLast24Hours = orderStats?.DeliveryRevenueLast24 ?? 0,
                PendingOrders = orderStats?.PendingOrders ?? 0,
                ConfirmedOrders = orderStats?.ConfirmedOrders ?? 0,
                ProcessingOrders = orderStats?.ProcessingOrders ?? 0,
                ShippedOrders = orderStats?.ShippedOrders ?? 0,
                DeliveredOrders = orderStats?.DeliveredOrders ?? 0,
                CancelledOrders = orderStats?.CancelledOrders ?? 0,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers
            };
        }
    }
}
