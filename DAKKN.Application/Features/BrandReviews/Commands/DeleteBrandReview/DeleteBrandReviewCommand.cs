using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview
{
    public record DeleteBrandReviewCommand(Guid Id, Guid UserId) : IRequest;
}
