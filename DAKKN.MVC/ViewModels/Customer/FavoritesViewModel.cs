using DAKKN.Application.DTOs;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class FavoritesViewModel
    {
        public List<ProductDto> FavoriteProducts { get; set; } = new();
    }
}
