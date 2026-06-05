using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "password_required")]
        [MinLength(8, ErrorMessage = "password_min_length")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "confirm_required")]
        [Compare(nameof(NewPassword), ErrorMessage = "passwords_mismatch")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
