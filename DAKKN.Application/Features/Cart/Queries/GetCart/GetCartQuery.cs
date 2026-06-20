using DAKKN.Application.Features.Cart.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Cart.Queries.GetCart
{
    public record GetCartQuery : IRequest<CartDto>;
}
