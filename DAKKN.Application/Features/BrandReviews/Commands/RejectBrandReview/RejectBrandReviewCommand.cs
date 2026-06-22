using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.RejectBrandReview
{
    public record RejectBrandReviewCommand(Guid Id) : IRequest;
}
