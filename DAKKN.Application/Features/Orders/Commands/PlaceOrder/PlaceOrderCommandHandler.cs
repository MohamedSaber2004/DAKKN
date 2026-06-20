using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestCartStorage _cartStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;

        public PlaceOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IGuestCartStorage cartStorage,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _cartStorage = cartStorage;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            var cartItems = _cartStorage.GetCart();
            if (cartItems.Count == 0)
                throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.Empty.Value]);

            var productRepo = _unitOfWork.GetRepository<Product>();
            var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction>();
            var resultItems = new List<OrderItemResult>();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var cartItem in cartItems)
                {
                    var product = await productRepo.FindByKeyAsync(cartItem.ProductId, cancellationToken);
                    if (product == null)
                        throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.ProductNotFound.Value]);

                    if (product.QuantityInStock < cartItem.Quantity)
                        throw new BadRequestException(
                            string.Format(_localizer[LocalizationKeys.Inventory.OnlyXAvailable.Value], product.QuantityInStock));

                    var previousQty = product.QuantityInStock;
                    product.ReduceStock(cartItem.Quantity);

                    productRepo.Update(product);

                    var transaction = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        QuantityChanged = -cartItem.Quantity,
                        PreviousQuantity = previousQty,
                        NewQuantity = product.QuantityInStock,
                        TransactionType = InventoryTransactionType.OrderPlaced,
                        Notes = $"Order placed for {cartItem.Quantity} units"
                    };
                    await transactionRepo.AddAsync(transaction);

                    resultItems.Add(new OrderItemResult
                    {
                        ProductId = product.Id,
                        ProductName = cartItem.Name,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price
                    });
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _cartStorage.ClearCart();

                return new PlaceOrderResult
                {
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Items = resultItems,
                    TotalAmount = resultItems.Sum(i => i.Subtotal),
                    OrderDate = DateTime.UtcNow
                };
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
