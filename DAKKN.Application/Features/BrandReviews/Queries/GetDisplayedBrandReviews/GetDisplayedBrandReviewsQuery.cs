using DAKKN.Application.Features.BrandReviews.DTOs;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews
{
    public record GetDisplayedBrandReviewsQuery : IRequest<List<BrandReviewDto>>;
}
