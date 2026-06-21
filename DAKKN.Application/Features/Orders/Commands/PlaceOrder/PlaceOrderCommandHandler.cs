using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestCartStorage _cartStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<PlaceOrderCommandHandler> _logger;

        public PlaceOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IGuestCartStorage cartStorage,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer,
            ILogger<PlaceOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cartStorage = cartStorage;
            _currentUser = currentUser;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            var cartItems = _cartStorage.GetCart();
            if (cartItems.Count == 0)
                throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.Empty.Value]);

            var productRepo = _unitOfWork.GetRepository<Product>();
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var orderItemRepo = _unitOfWork.GetRepository<OrderItem>();
            var historyRepo = _unitOfWork.GetRepository<OrderStatusHistory>();
            var transactionRepo = _unitOfWork.GetRepository<InventoryTransaction>();

            var governorateRepo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var governorate = await governorateRepo.FindByKeyAsync(request.ShippingGovernorateId, cancellationToken);
            if (governorate == null)
                throw new BadRequestException(_localizer[LocalizationKeys.ShippingMessages.NotFound.Value]);

            var userId = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null;
            var userEmail = string.Empty;
            if (_currentUser.IsAuthenticated)
            {
                var userRepo = _unitOfWork.GetRepository<ApplicationUser>();
                var user = await userRepo.FindByKeyAsync(_currentUser.UserId, cancellationToken);
                if (user != null) userEmail = user.Email ?? string.Empty;
            }

            decimal subtotal = 0;
            var items = new List<(Product product, int quantity)>();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var cartItem in cartItems)
                {
                    var product = await productRepo.FindByKeyAsync(cartItem.ProductId, cancellationToken);
                    if (product == null || product.IsDeleted)
                        throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.ProductNotFound.Value]);

                    if (!product.IsActive)
                        throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.ProductNotAvailable.Value]);

                    if (product.QuantityInStock < cartItem.Quantity)
                        throw new BadRequestException(
                            string.Format(_localizer[LocalizationKeys.Inventory.OnlyXAvailable.Value], product.QuantityInStock));

                    items.Add((product, cartItem.Quantity));
                    subtotal += product.Price * cartItem.Quantity;
                }

                var order = new Order(
                    request.CustomerName,
                    userEmail,
                    request.CustomerPhone,
                    request.ShippingAddress,
                    request.ShippingGovernorateId,
                    governorate.Name,
                    governorate.ShippingPrice,
                    subtotal,
                    userId,
                    request.Notes);

                await orderRepo.AddAsync(order);

                foreach (var (product, quantity) in items)
                {
                    var orderItem = new OrderItem(
                        order.Id, product.Id, product.Name, product.ImageUrl, product.Price, quantity);
                    await orderItemRepo.AddAsync(orderItem);

                    var previousQty = product.QuantityInStock;
                    product.ReduceStock(quantity);
                    productRepo.Update(product);

                    var transaction = new InventoryTransaction
                    {
                        ProductId = product.Id,
                        QuantityChanged = -quantity,
                        PreviousQuantity = previousQty,
                        NewQuantity = product.QuantityInStock,
                        TransactionType = InventoryTransactionType.OrderPlaced,
                        Notes = $"Order {order.OrderNumber}: {quantity} units"
                    };
                    await transactionRepo.AddAsync(transaction);
                }

                var changedBy = _currentUser.IsAuthenticated ? _currentUser.UserId.ToString() : "System";
                var history = new OrderStatusHistory(
                    order.Id, order.Status, order.Status, changedBy, "Order created");
                await historyRepo.AddAsync(history);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _cartStorage.ClearCart();

                _logger.LogInformation("Order {OrderNumber} created by user {UserId}", order.OrderNumber, userId);

                return new PlaceOrderResult
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    TrackingNumber = order.TrackingNumber,
                    TotalAmount = order.TotalAmount,
                    OrderDate = order.CreatedAt,
                    ItemCount = items.Count
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
