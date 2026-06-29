namespace DAKKN.Application.Features.StickerSuggestions.DTOs
{
    public record SubmitSuggestionResponseDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
    }
}
