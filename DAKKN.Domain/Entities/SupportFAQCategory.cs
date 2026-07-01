using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportFAQCategory : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public static SupportFAQCategory Create(string name, string arName, string? icon = null, int displayOrder = 0)
        {
            return new SupportFAQCategory
            {
                Id = Guid.NewGuid(),
                Name = name,
                ArName = arName,
                Icon = icon,
                DisplayOrder = displayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
