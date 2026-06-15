using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "alert.email_required")]
        [EmailAddress(ErrorMessage = "alert.email_invalid")]
        public string Email { get; set; } = string.Empty;
    }
}
