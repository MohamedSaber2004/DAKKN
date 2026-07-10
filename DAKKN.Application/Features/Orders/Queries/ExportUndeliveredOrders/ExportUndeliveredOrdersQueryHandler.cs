using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.ExportUndeliveredOrders
{
    public class ExportUndeliveredOrdersQueryHandler : IRequestHandler<ExportUndeliveredOrdersQuery, List<ExportUndeliveredOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExportUndeliveredOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExportUndeliveredOrderDto>> Handle(ExportUndeliveredOrdersQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var undeliveredStatuses = new[]
            {
                OrderStatus.Pending,
                OrderStatus.Confirmed,
                OrderStatus.Processing,
                OrderStatus.Packed,
                OrderStatus.Shipped,
                OrderStatus.OutForDelivery
            };

            var orders = await repo.GetAllAsync(null).AsNoTracking()
                .Where(o => !o.IsDeleted && undeliveredStatuses.Contains(o.Status))
                .OrderBy(o => o.CreatedAt)
                .Select(o => new ExportUndeliveredOrderDto
                {
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    ShippingAddress = o.ShippingAddress,
                    OrderNumber = o.OrderNumber,
                    TrackingNumber = o.TrackingNumber,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
