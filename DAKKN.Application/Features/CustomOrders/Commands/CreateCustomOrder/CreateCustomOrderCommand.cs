using DAKKN.Application.Features.CustomOrders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Commands.CreateCustomOrder
{
    public record CreateCustomOrderCommand(
        string CustomerName,
        string CustomerPhone,
        string ShippingAddress,
        string? Notes,
        string? ImageUrl,
        string? Shape,
        string? Size,
        int Quantity,
        decimal TotalAmount
    ) : IRequest<CustomOrderDetailDto>;
}
