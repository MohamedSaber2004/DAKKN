using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.ToggleDisplayBrandReview
{
    public record ToggleDisplayBrandReviewCommand(Guid Id) : IRequest;
}
