using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.UpdateBrandReview
{
    public class UpdateBrandReviewCommandHandler : IRequestHandler<UpdateBrandReviewCommand, BrandReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateBrandReviewCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<BrandReviewDto> Handle(UpdateBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var review = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (review is null || review.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            if (review.UserId != request.UserId)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.ExceptionMessages.Unauthorized.Value]);

            review.Rating = request.Rating;
            review.ReviewTitle = request.ReviewTitle;
            review.ReviewText = request.ReviewText;

            repo.Update(review);
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
                UpdatedAt = review.UpdatedAt,
                ApprovedAt = review.ApprovedAt
            };
        }
    }
}
