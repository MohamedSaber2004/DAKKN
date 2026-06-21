using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.GetRecentOrders
{
    public class GetRecentOrdersQueryHandler : IRequestHandler<GetRecentOrdersQuery, List<RecentOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RecentOrderDto>> Handle(GetRecentOrdersQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var orders = await repo.GetAllAsync(null).AsNoTracking()
                .Where(o => !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .Take(request.Count)
                .Select(o => new RecentOrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.CustomerName,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
