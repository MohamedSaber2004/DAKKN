namespace DAKKN.Application.DTOs
{
    public class ShippingGovernorateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public decimal ShippingPrice { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
