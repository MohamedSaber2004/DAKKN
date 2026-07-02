namespace DAKKN.Application.DTOs
{
    public class UserSettingsDto
    {
        public string? Language { get; set; }
        public string Theme { get; set; } = "light";
        public bool IsDarkMode { get; set; }
        public string LayoutMode { get; set; } = "default";
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
