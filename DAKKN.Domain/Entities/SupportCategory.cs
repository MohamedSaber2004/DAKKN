using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SupportCategory : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<SupportTicket> Tickets { get; set; } = new List<SupportTicket>();
        public ICollection<SupportFAQ> FAQs { get; set; } = new List<SupportFAQ>();

        public static SupportCategory Create(string name, string arName, string? description = null, string? icon = null, int displayOrder = 0)
        {
            return new SupportCategory
            {
                Id = Guid.NewGuid(),
                Name = name,
                ArName = arName,
                Description = description,
                Icon = icon,
                DisplayOrder = displayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
