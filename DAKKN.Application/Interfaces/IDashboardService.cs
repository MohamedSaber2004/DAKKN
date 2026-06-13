using System.Threading.Tasks;
using DAKKN.Application.DTOs;

namespace DAKKN.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetCustomerDashboardDataAsync(Guid customerId);
    }
}
