using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "alert.email_required")]
        [EmailAddress(ErrorMessage = "alert.email_invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "alert.password_required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
