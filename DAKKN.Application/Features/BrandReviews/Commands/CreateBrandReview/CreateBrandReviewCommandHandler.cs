using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.BrandReviews.Commands.CreateBrandReview
{
    public class CreateBrandReviewCommandHandler : IRequestHandler<CreateBrandReviewCommand, BrandReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBrandReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BrandReviewDto> Handle(CreateBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new BrandReview
            {
                UserId = request.UserId,
                Rating = request.Rating,
                ReviewTitle = request.ReviewTitle,
                ReviewText = request.ReviewText,
                IsApproved = false,
                IsDisplayed = false,
                DisplayOrder = 0
            };

            var repo = _unitOfWork.GetRepository<BrandReview>();
            await repo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new BrandReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                Rating = review.Rating,
                ReviewTitle = review.ReviewTitle,
                ReviewText = review.ReviewText,
                IsApproved = review.IsApproved,
                IsDisplayed = review.IsDisplayed,
                DisplayOrder = review.DisplayOrder,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }
    }
}
