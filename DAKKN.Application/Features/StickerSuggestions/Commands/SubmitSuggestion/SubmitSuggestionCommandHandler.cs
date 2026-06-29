using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion
{
    public class SubmitSuggestionCommandHandler : IRequestHandler<SubmitSuggestionCommand, SubmitSuggestionResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageValidator _imageValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public SubmitSuggestionCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IImageValidator imageValidator,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _imageValidator = imageValidator;
            _localizer = localizer;
        }

        public async Task<SubmitSuggestionResponseDto> Handle(SubmitSuggestionCommand request, CancellationToken cancellationToken)
        {
            string? imagePath = null;

            if (request.ReferenceImage is not null)
            {
                var (uploaded, result) = await _imageValidator.UploadImage(request.ReferenceImage, 6);
                if (!uploaded)
                    throw new BadRequestException(_localizer[LocalizationKeys.UploadFileMessages.FileUploadFailed.Value]);
                imagePath = result;
            }

            var suggestion = StickerSuggestion.Create(
                _currentUserService.UserId,
                request.Title,
                request.Description,
                imagePath,
                request.Tags);

            var repo = _unitOfWork.GetRepository<StickerSuggestion>();
            await repo.AddAsync(suggestion);
            await _unitOfWork.SaveChangesAsync();

            return new SubmitSuggestionResponseDto
            {
                Id = suggestion.Id,
                Title = suggestion.Title,
                Status = suggestion.Status.ToString()
            };
        }
    }
}
