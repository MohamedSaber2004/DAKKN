using System;
using System.Threading.Tasks;
using DAKKN.Application.DTOs;

namespace DAKKN.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(Guid id);
    }
}
