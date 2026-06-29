using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetAllSuggestions
{
    public class GetAllSuggestionsQueryHandler : IRequestHandler<GetAllSuggestionsQuery, PagginatedResult<StickerSuggestionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSuggestionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagginatedResult<StickerSuggestionDto>> Handle(GetAllSuggestionsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<StickerSuggestion>();
            var query = repo.GetAllAsync(s => !s.IsDeleted)
                .AsNoTracking()
                .Include(s => s.User)
                .Include(s => s.Product)
                .AsQueryable();

            if (request.Status.HasValue)
                query = query.Where(s => s.Status == request.Status.Value);

            query = query.OrderByDescending(s => s.CreatedAt);

            var projected = query.Select(s => new StickerSuggestionDto
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

            return await projected.AsPagginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
