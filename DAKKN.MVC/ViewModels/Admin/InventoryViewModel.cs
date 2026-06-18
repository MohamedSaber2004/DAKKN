using DAKKN.Application.DTOs;
using System;
using System.Collections.Generic;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class InventoryViewModel
    {
        public List<ProductDto> Products { get; set; } = new();
        public int TotalProducts { get; set; }
    }
}
