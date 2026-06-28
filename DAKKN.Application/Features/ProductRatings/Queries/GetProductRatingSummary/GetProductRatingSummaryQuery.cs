using DAKKN.Application.Features.ProductRatings.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ProductRatings.Queries.GetProductRatingSummary
{
    public class GetProductRatingSummaryQuery : IRequest<ProductRatingSummaryDto>
    {
        public Guid ProductId { get; set; }
    }
}
