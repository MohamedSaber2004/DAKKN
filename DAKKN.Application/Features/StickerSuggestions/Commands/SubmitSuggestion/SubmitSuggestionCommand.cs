using DAKKN.Application.Features.StickerSuggestions.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion
{
    public record SubmitSuggestionCommand : IRequest<SubmitSuggestionResponseDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Tags { get; set; }
        public IFormFile? ReferenceImage { get; set; }
    }
}
