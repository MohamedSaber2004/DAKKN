namespace DAKKN.MVC.ViewModels.Shared
{
    public class PasswordInputModel
    {
        public string For      { get; set; } = string.Empty;  // e.g. "Password"
        public string LabelKey { get; set; } = string.Empty;  // i18n key
        public string PlaceholderKey { get; set; } = string.Empty; // i18n key for placeholder
        public string? LeadingIcon { get; set; } // material icon name
    }
}
