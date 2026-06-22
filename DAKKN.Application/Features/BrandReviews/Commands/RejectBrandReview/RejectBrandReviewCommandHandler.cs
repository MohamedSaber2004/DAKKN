using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.RejectBrandReview
{
    public class RejectBrandReviewCommandHandler : IRequestHandler<RejectBrandReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public RejectBrandReviewCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task Handle(RejectBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var review = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (review is null || review.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            review.IsApproved = false;
            review.IsDisplayed = false;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
