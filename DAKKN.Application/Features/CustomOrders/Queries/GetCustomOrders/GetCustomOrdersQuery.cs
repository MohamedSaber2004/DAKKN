using DAKKN.Application.Features.CustomOrders.DTOs;
using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrders
{
    public record GetCustomOrdersQuery(
        CustomOrderStatus? FilterStatus = null,
        int Page = 1,
        int PageSize = 20
    ) : IRequest<CustomOrderListResultDto>;

    public class CustomOrderListResultDto
    {
        public List<CustomOrderListDto> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
