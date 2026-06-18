using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class UserSettings : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string Language { get; set; } = "ar";
        public string Theme { get; set; } = "light";
        public string PrimaryColor { get; set; } = "#3B82F6"; // Default blue
        public bool IsDarkMode { get; set; }
        public string LayoutMode { get; set; } = "default";

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
