namespace DAKKN.Application.Features.Auth.DTOs
{
    public record AuthResponseDto(
    string? AccessToken,
    string? RefreshToken,
    string FullName,
    string Email,
    Guid UserId,
    IEnumerable<string> Roles);

    public record RefreshTokenResponseDto(
        string AccessToken,
        string RefreshToken,
        Guid UserId);
}
