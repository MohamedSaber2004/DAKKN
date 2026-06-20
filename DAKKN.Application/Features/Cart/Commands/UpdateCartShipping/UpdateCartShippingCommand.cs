using DAKKN.Application.Features.Cart.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartShipping
{
    public record UpdateCartShippingCommand(Guid ShippingGovernorateId) : IRequest<CartDto>;
}
