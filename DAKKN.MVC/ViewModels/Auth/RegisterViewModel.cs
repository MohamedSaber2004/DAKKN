using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "name_required")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "email_required")]
        [EmailAddress(ErrorMessage = "email_invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "password_required")]
        [MinLength(8, ErrorMessage = "password_min_length")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "confirm_required")]
        [Compare(nameof(Password), ErrorMessage = "passwords_mismatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
