using DAKKN.Application.Features.BrandReviews.DTOs;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.UpdateBrandReview
{
    public record UpdateBrandReviewCommand(
        Guid Id,
        Guid UserId,
        int Rating,
        string ReviewTitle,
        string ReviewText
    ) : IRequest<BrandReviewDto>;
}
