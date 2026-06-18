using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin;

public class AdminSettingsViewModel
{
    [Required(ErrorMessage = "validation.required")]
    [Display(Name = "admin_user_full_name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "auth.login.email")]
    public string Email { get; set; } = string.Empty;

    public string Language { get; set; } = "ar";
    public string Theme { get; set; } = "light";
    public string PrimaryColor { get; set; } = "#3B82F6";
    public bool IsDarkMode { get; set; }
    public string LayoutMode { get; set; } = "default";

    // Profile picture – filename only (e.g. "320260618123000001.png")
    public string? ProfilePictureUrl { get; set; }

    /// <summary>True when the admin currently has a saved profile picture.</summary>
    public bool HasProfileImage => !string.IsNullOrWhiteSpace(ProfilePictureUrl);

    /// <summary>Relative URL used by &lt;img&gt; tags. Returns the /files/ endpoint path or an empty string.</summary>
    public string ProfileImageSrc =>
        HasProfileImage ? $"/files/{ProfilePictureUrl}" : string.Empty;
}

