using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus
{
    public record UpdateSuggestionStatusCommand : IRequest<string>
    {
        public Guid SuggestionId { get; set; }
        public SuggestionStatus NewStatus { get; set; }
        public string? AdminNote { get; set; }
        public Guid? ConvertedProductId { get; set; }
    }
}
