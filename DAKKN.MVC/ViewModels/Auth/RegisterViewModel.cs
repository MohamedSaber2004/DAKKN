using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "alert.name_required")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.email_required")]
        [EmailAddress(ErrorMessage = "alert.email_invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.phone_required")]
        [Phone(ErrorMessage = "alert.phone_invalid")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.password_required")]
        [MinLength(8, ErrorMessage = "alert.password_min_length")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.confirm_required")]
        [Compare(nameof(Password), ErrorMessage = "alert.passwords_mismatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
