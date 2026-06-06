using System.Collections.Generic;

namespace DAKKN.Application.DTOs
{
    public class DashboardDto
    {
        public OrderDto? LastOrder { get; set; }
        public List<ProductDto> Recommendations { get; set; } = new();
        public List<ProductDto> ProgrammingStickers { get; set; } = new();
        public List<ProductDto> MemeStickers { get; set; } = new();
    }
}
