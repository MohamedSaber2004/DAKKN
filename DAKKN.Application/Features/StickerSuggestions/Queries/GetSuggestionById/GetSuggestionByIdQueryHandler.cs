using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById
{
    public class GetSuggestionByIdQueryHandler : IRequestHandler<GetSuggestionByIdQuery, StickerSuggestionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetSuggestionByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<StickerSuggestionDto> Handle(GetSuggestionByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<StickerSuggestion>();
            var suggestion = await repo.GetAllAsync(s => s.Id == request.Id && !s.IsDeleted)
                .AsNoTracking()
                .Include(s => s.User)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(cancellationToken);

            if (suggestion is null)
                throw new NotFoundException(_localizer[LocalizationKeys.SuggestionMessages.NotFound.Value]);

            var isAdmin = _currentUserService.UserType == Domain.Enums.UserType.Admin;
            if (suggestion.UserId != _currentUserService.UserId && !isAdmin)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.SuggestionMessages.Unauthorized.Value]);

            return new StickerSuggestionDto
            {
                Id = suggestion.Id,
                UserId = suggestion.UserId,
                UserEmail = suggestion.User.Email ?? string.Empty,
                Title = suggestion.Title,
                Description = suggestion.Description,
                ReferenceImagePath = suggestion.ReferenceImagePath,
                Tags = suggestion.Tags,
                Status = suggestion.Status.ToString(),
                AdminNote = suggestion.AdminNote,
                ConvertedProductId = suggestion.ConvertedProductId,
                ConvertedProductName = suggestion.Product?.Name,
                ConvertedProductImageUrl = suggestion.Product?.ImageUrl,
                CreatedAt = suggestion.CreatedAt
            };
        }
    }
}
