using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.GetOrders
{
    public record GetOrdersQuery(
        string? SearchTerm = null,
        OrderStatus? FilterStatus = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<AdminOrderListResultDto>;

    public class AdminOrderListResultDto
    {
        public List<OrderListDto> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PendingCount { get; set; }
        public int ProcessingCount { get; set; }
        public int ShippedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }
}
