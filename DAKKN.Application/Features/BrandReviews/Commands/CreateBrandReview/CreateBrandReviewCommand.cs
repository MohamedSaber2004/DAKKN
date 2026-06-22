using DAKKN.Application.Features.BrandReviews.DTOs;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.CreateBrandReview
{
    public class CreateBrandReviewCommand : IRequest<BrandReviewDto>
    {
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string ReviewTitle { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
    }
}
