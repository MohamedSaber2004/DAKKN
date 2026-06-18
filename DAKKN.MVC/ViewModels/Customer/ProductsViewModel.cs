using DAKKN.Application.DTOs;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class ProductsViewModel
    {
        public List<ProductDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
