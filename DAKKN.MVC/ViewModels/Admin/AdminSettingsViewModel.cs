using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin;

public class AdminSettingsViewModel
{
    [Required(ErrorMessage = "admin_first_name_required")]
    [Display(Name = "admin_first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "admin_last_name_required")]
    [Display(Name = "admin_last_name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "auth.login.email")]
    public string Email { get; set; } = string.Empty;

    // You can add more settings here like Theme preference if you want to store it in DB
    // public string Theme { get; set; } = "light";
}
