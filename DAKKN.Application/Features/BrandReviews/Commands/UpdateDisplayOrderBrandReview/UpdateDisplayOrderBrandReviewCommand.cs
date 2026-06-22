using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.UpdateDisplayOrderBrandReview
{
    public record UpdateDisplayOrderBrandReviewCommand(Guid Id, int DisplayOrder) : IRequest;
}
