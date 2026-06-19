using DAKKN.Application.DTOs;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class CategoryProductsViewModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
        public List<ProductDto> Products { get; set; } = new();
        public decimal AveragePrice { get; set; }
        public int ActiveProducts { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
