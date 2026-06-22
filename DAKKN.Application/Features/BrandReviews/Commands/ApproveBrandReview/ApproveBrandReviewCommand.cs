using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.ApproveBrandReview
{
    public record ApproveBrandReviewCommand(Guid Id, Guid AdminId) : IRequest;
}
