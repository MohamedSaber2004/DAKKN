using DAKKN.Application.Common.Models;
using DAKKN.Application.Features.StickerSuggestions.DTOs;
using MediatR;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions
{
    public record GetMySuggestionsQuery(
        int PageNumber = PagginatedResult<StickerSuggestionDto>.DefaultPageNumber,
        int PageSize = PagginatedResult<StickerSuggestionDto>.DefaultPageSize
    ) : IRequest<PagginatedResult<StickerSuggestionDto>>;
}
