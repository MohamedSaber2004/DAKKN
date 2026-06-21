using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;

        public CancelOrderCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var orderItemRepo = _unitOfWork.GetRepository<OrderItem>();
            var historyRepo = _unitOfWork.GetRepository<OrderStatusHistory>();
            var productRepo = _unitOfWork.GetRepository<Product>();
            var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction>();

            var order = await orderRepo.FindByKeyAsync(request.OrderId, cancellationToken);
            if (order == null || order.IsDeleted)
                throw new NotFoundException(nameof(Order), request.OrderId);

            if (!order.CanCancel())
                throw new BadRequestException(_localizer[LocalizationKeys.OrderMessages.CannotCancel.Value]);

            var changedBy = _currentUser.IsAuthenticated ? _currentUser.UserId.ToString() : "System";

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var oldStatus = order.Status;
                order.Cancel(changedBy);
                orderRepo.Update(order);

                var history = new OrderStatusHistory(
                    order.Id, oldStatus, OrderStatus.Cancelled, changedBy, request.Reason);
                await historyRepo.AddAsync(history);

                var orderItems = orderItemRepo.GetBy(oi => oi.OrderId == order.Id);
                foreach (var item in orderItems)
                {
                    var product = await productRepo.FindByKeyAsync(item.ProductId, cancellationToken);
                    if (product != null && !product.IsDeleted)
                    {
                        var previousQty = product.QuantityInStock;
                        product.IncreaseStock(item.Quantity);
                        productRepo.Update(product);

                        var transaction = new InventoryTransaction
                        {
                            ProductId = product.Id,
                            QuantityChanged = item.Quantity,
                            PreviousQuantity = previousQty,
                            NewQuantity = product.QuantityInStock,
                            TransactionType = InventoryTransactionType.OrderCancelled,
                            Notes = $"Cancelled Order {order.OrderNumber}: {item.Quantity} units restored"
                        };
                        await transactionRepo.AddAsync(transaction);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
