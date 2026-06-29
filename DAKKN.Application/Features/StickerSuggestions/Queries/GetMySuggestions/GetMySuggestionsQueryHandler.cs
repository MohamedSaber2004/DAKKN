using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions
{
    public class GetMySuggestionsQueryHandler : IRequestHandler<GetMySuggestionsQuery, PagginatedResult<StickerSuggestionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetMySuggestionsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PagginatedResult<StickerSuggestionDto>> Handle(GetMySuggestionsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<StickerSuggestion>();
            var query = repo.GetAllAsync(s => s.UserId == _currentUserService.UserId && !s.IsDeleted)
                .AsNoTracking()
                .Include(s => s.User)
                .Include(s => s.Product)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new StickerSuggestionDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    UserEmail = s.User.Email ?? string.Empty,
                    Title = s.Title,
                    Description = s.Description,
                    ReferenceImagePath = s.ReferenceImagePath,
                    Tags = s.Tags,
                    Status = s.Status.ToString(),
                    AdminNote = s.AdminNote,
                    ConvertedProductId = s.ConvertedProductId,
                    ConvertedProductName = s.Product != null ? s.Product.Name : null,
                    ConvertedProductImageUrl = s.Product != null ? s.Product.ImageUrl : null,
                    CreatedAt = s.CreatedAt
                });

            return await query.AsPagginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
