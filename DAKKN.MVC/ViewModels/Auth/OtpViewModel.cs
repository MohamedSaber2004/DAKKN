using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Auth
{
    public class OtpViewModel
    {
        public string Email   { get; set; } = string.Empty;
        public string Purpose { get; set; } = "VerifyEmail"; // "VerifyEmail" | "ForgotPassword"

        // Individual digit inputs — joined in the Code property
        [Required(ErrorMessage = "alert.validation.required")] public string Digit1 { get; set; } = string.Empty;
        [Required(ErrorMessage = "alert.validation.required")] public string Digit2 { get; set; } = string.Empty;
        [Required(ErrorMessage = "alert.validation.required")] public string Digit3 { get; set; } = string.Empty;
        [Required(ErrorMessage = "alert.validation.required")] public string Digit4 { get; set; } = string.Empty;
        [Required(ErrorMessage = "alert.validation.required")] public string Digit5 { get; set; } = string.Empty;
        [Required(ErrorMessage = "alert.validation.required")] public string Digit6 { get; set; } = string.Empty;

        public string Code => Digit1 + Digit2 + Digit3 + Digit4 + Digit5 + Digit6;
    }
}
