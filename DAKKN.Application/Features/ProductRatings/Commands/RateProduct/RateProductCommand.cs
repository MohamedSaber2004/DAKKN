using DAKKN.Application.Features.ProductRatings.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ProductRatings.Commands.RateProduct
{
    public class RateProductCommand : IRequest<ProductRatingSummaryDto>
    {
        public Guid ProductId { get; set; }
        public int Stars { get; set; }
    }
}
