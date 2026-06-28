using System.Collections.Generic;

namespace DAKKN.Application.DTOs
{
    public class DashboardDto
    {
        public OrderDto? LastOrder { get; set; }
        public List<ProductDto> Recommendations { get; set; } = new();
        public int TotalOrders { get; set; }
        public int TotalFavorites { get; set; }
    }
}
