using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.Orders.DTOs
{
    public class DashboardStatsDto
    {
        public int OrdersToday { get; set; }
        public int OrdersLast24Hours { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueLast24Hours { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
    }

    public class RecentOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
