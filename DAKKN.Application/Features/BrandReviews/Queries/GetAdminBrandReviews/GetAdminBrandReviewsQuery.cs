using DAKKN.Application.Features.BrandReviews.DTOs;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetAdminBrandReviews
{
    public record GetAdminBrandReviewsQuery(
        string? StatusFilter,
        int? RatingFilter,
        string? SearchTerm,
        string? SortBy
    ) : IRequest<List<BrandReviewDto>>;
}
