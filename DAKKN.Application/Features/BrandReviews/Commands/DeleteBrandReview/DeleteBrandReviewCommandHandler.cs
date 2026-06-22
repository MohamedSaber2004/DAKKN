using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview
{
    public class DeleteBrandReviewCommandHandler : IRequestHandler<DeleteBrandReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteBrandReviewCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task Handle(DeleteBrandReviewCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var review = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (review is null || review.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.BrandReviews.NotFound.Value]);

            if (review.UserId != request.UserId)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.ExceptionMessages.Unauthorized.Value]);

            review.MarkAsDeleted(request.UserId.ToString());
            repo.Update(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
