using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.UpdateDisplayOrderBrandReview
{
    public class UpdateDisplayOrderBrandReviewCommandHandler : IRequestHandler<UpdateDisplayOrderBrandReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateDisplayOrderBrandReviewCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task Handle(UpdateDisplayOrderBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var review = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (review is null || review.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            review.DisplayOrder = request.DisplayOrder;

            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
