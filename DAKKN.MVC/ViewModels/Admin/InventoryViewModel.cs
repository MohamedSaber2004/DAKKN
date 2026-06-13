using System.Collections.Generic;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class InventoryProductViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockLevel { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsLowStock => StockLevel < 10;
    }

    public class InventoryViewModel
    {
        public List<InventoryProductViewModel> Products { get; set; } = new();
        public int TotalProducts { get; set; }
        public int LowStockAlerts { get; set; }
        public int CategoriesCount { get; set; }
        public string TotalStockValue { get; set; } = "0 ج.م";
    }
}
