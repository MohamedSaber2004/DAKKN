using DAKKN.Application.Features.CustomOrders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.CustomOrders.Queries.ExportUndeliveredCustomOrders
{
    public class ExportUndeliveredCustomOrdersQueryHandler : IRequestHandler<ExportUndeliveredCustomOrdersQuery, List<ExportUndeliveredCustomOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExportUndeliveredCustomOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExportUndeliveredCustomOrderDto>> Handle(ExportUndeliveredCustomOrdersQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<CustomOrder>();

            var undeliveredStatuses = new[]
            {
                CustomOrderStatus.Pending
            };

            var orders = await repo.GetAllAsync(null).AsNoTracking()
                .Where(o => !o.IsDeleted && undeliveredStatuses.Contains(o.Status))
                .OrderBy(o => o.CreatedAt)
                .Select(o => new ExportUndeliveredCustomOrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.CustomerPhone,
                    ShippingAddress = o.ShippingAddress,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
