using DAKKN.Application.Features.StickerSuggestions.DTOs;
using MediatR;

namespace DAKKN.Application.Features.StickerSuggestions.Queries.GetSuggestionById
{
    public record GetSuggestionByIdQuery(Guid Id) : IRequest<StickerSuggestionDto>;
}
