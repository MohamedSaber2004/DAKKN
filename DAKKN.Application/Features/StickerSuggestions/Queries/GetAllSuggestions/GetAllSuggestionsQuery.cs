using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetAllSuggestions
{
    public record GetAllSuggestionsQuery(
        int PageNumber = PagginatedResult<StickerSuggestionDto>.DefaultPageNumber,
        int PageSize = PagginatedResult<StickerSuggestionDto>.DefaultPageSize,
        SuggestionStatus? Status = null
    ) : IRequest<PagginatedResult<StickerSuggestionDto>>;
}
