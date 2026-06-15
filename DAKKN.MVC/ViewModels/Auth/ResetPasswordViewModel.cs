using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.password_required")]
        [MinLength(8, ErrorMessage = "alert.password_min_length")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.confirm_required")]
        [Compare(nameof(NewPassword), ErrorMessage = "alert.passwords_mismatch")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
