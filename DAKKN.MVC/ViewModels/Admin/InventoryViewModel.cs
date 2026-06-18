using DAKKN.Application.DTOs;
using System;
using System.Collections.Generic;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class InventoryViewModel
    {
        public List<ProductDto> Products { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public int TotalProducts { get; set; }
        public Guid? SelectedCategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
