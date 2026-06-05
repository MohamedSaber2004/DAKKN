using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "email_required")]
        [EmailAddress(ErrorMessage = "email_invalid")]
        public string Email { get; set; } = string.Empty;
    }
}
