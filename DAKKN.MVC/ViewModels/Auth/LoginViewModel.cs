using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "email_required")]
        [EmailAddress(ErrorMessage = "email_invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "password_required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
