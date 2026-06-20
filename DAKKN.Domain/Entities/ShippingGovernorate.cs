using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class ShippingGovernorate : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public decimal ShippingPrice { get; set; }
        public int DisplayOrder { get; set; }
    }
}
