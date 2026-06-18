namespace DAKKN.MVC.ViewModels.Customer;

public class CustomerProfileViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }

    public bool HasProfileImage => !string.IsNullOrWhiteSpace(ProfilePictureUrl);

    public string ProfileImageSrc =>
        HasProfileImage ? $"/files/{ProfilePictureUrl}" : string.Empty;
}
