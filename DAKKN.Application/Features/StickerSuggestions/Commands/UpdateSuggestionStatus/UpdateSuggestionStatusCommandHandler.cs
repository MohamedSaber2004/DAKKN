using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus
{
    public class UpdateSuggestionStatusCommandHandler : IRequestHandler<UpdateSuggestionStatusCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateSuggestionStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<string> Handle(UpdateSuggestionStatusCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<StickerSuggestion>();
            var suggestion = await repo.FindByKeyAsync(request.SuggestionId, cancellationToken);
            if (suggestion is null)
                throw new NotFoundException(_localizer[LocalizationKeys.SuggestionMessages.NotFound.Value]);

            if (suggestion.Status == SuggestionStatus.Rejected && request.NewStatus == SuggestionStatus.Approved)
                throw new BadRequestException(_localizer[LocalizationKeys.SuggestionMessages.InvalidStatusTransition.Value]);

            var adminId = _currentUserService.UserId;

            switch (request.NewStatus)
            {
                case SuggestionStatus.UnderReview:
                    suggestion.MarkUnderReview(adminId);
                    break;
                case SuggestionStatus.Approved:
                    suggestion.Approve(request.AdminNote, adminId);
                    break;
                case SuggestionStatus.Rejected:
                    suggestion.Reject(request.AdminNote, adminId);
                    break;
                default:
                    throw new BadRequestException(_localizer[LocalizationKeys.ValidationMessages.InvalidEnum.Value]);
            }

            if (request.ConvertedProductId.HasValue && request.NewStatus == SuggestionStatus.Approved)
                suggestion.LinkProduct(request.ConvertedProductId.Value, adminId);

            repo.Update(suggestion);
            await _unitOfWork.SaveChangesAsync();

            return _localizer[LocalizationKeys.SuggestionMessages.StatusUpdated.Value];
        }
    }
}
