using DAKKN.Application.Features.BrandReviews.DTOs;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetCustomerBrandReviews
{
    public record GetCustomerBrandReviewsQuery(Guid UserId) : IRequest<List<BrandReviewDto>>;
}
