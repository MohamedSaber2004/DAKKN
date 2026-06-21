using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.GetCustomerOrders
{
    public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, List<OrderListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetCustomerOrdersQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<List<OrderListDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated)
                throw new UnAuthorizedException();

            var repo = _unitOfWork.GetRepository<Order>();

            var orders = await repo.GetAllAsync(null).AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.UserId == _currentUser.UserId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    TrackingNumber = o.TrackingNumber,
                    CustomerName = o.CustomerName,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.Items.Count
                })
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
