using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.ToggleDisplayBrandReview
{
    public class ToggleDisplayBrandReviewCommandHandler : IRequestHandler<ToggleDisplayBrandReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public ToggleDisplayBrandReviewCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task Handle(ToggleDisplayBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var review = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (review is null || review.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            if (!review.IsApproved)
                throw new BadRequestException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            if (review.IsDisplayed)
            {
                review.IsDisplayed = false;
                review.DisplayOrder = 0;
            }
            else
            {
                var displayedCount = await repo.GetAllAsync(r => r.IsDisplayed && !r.IsDeleted && r.Id != request.Id)
                    .CountAsync(cancellationToken);

                if (displayedCount >= 3)
                    throw new BadRequestException(_localizer[LocalizationKeys.BrandReviews.MaxDisplayedReached.Value]);

                var maxOrder = await repo.GetAllAsync(r => r.IsDisplayed && !r.IsDeleted)
                    .MaxAsync(r => (int?)r.DisplayOrder, cancellationToken) ?? 0;

                review.IsDisplayed = true;
                review.DisplayOrder = maxOrder + 1;
            }

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
