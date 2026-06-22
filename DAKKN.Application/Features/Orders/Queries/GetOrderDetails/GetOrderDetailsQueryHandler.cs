using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Orders.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Orders.Queries.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetOrderDetailsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<OrderDetailDto> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Order>();

            var query = repo.GetAllAsync(null).AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.StatusHistories)
                .Where(o => o.Id == request.OrderId && !o.IsDeleted);

            if (!request.IsAdmin)
            {
                if (!_currentUser.IsAuthenticated)
                    throw new UnAuthorizedException();
                query = query.Where(o => o.UserId == _currentUser.UserId);
            }

            var order = await query.FirstOrDefaultAsync(cancellationToken);
            if (order == null)
                throw new NotFoundException(nameof(Order), request.OrderId);

            return new OrderDetailDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TrackingNumber = order.TrackingNumber,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                ShippingGovernorateName = order.ShippingGovernorateName,
                ShippingCost = order.ShippingCost,
                Subtotal = order.Subtotal,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImageUrl = string.IsNullOrEmpty(i.ProductImageUrl)
                        ? null
                        : i.ProductImageUrl.StartsWith("http") || i.ProductImageUrl.StartsWith("/")
                            ? i.ProductImageUrl
                            : $"/files/{Path.GetFileName(i.ProductImageUrl)}",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                StatusHistory = order.StatusHistories
                    .OrderByDescending(h => h.CreatedAt)
                    .Select(h => new OrderStatusHistoryDto
                    {
                        OldStatus = h.OldStatus,
                        NewStatus = h.NewStatus,
                        ChangedBy = h.ChangedBy,
                        ChangedAt = h.CreatedAt,
                        Notes = h.Notes
                    }).ToList()
            };
        }
    }
}
