using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class SystemSetting : BaseEntity<Guid>
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
