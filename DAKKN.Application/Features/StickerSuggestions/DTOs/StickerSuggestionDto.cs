namespace DAKKN.Application.Features.StickerSuggestions.DTOs
{
    public record StickerSuggestionDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string UserEmail { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? ReferenceImagePath { get; init; }
        public string? Tags { get; init; }
        public string Status { get; init; } = string.Empty;
        public string? AdminNote { get; init; }
        public Guid? ConvertedProductId { get; init; }
        public string? ConvertedProductName { get; init; }
        public string? ConvertedProductImageUrl { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
