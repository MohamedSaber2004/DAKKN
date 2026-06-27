using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestCartStorage _cartStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public AddToCartCommandHandler(IUnitOfWork unitOfWork, IGuestCartStorage cartStorage, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _cartStorage = cartStorage;
            _localizer = localizer;
        }

        public async Task<int> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var product = await repo.FindByKeyAsync(request.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.ProductId);

            if (!product.IsActive || product.IsDeleted)
                throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.ProductNotAvailable]);

            if (product.QuantityInStock <= 0)
                throw new BadRequestException(_localizer[LocalizationKeys.CartMessages.OutOfStock]);

            var cart = _cartStorage.GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            var currentCartQty = existing?.Quantity ?? 0;
            var requestedTotal = currentCartQty + request.Quantity;

            if (requestedTotal > product.QuantityInStock)
                throw new BadRequestException(string.Format(_localizer[LocalizationKeys.CartMessages.OnlyAvailable], product.QuantityInStock));

            if (existing != null)
            {
                existing.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    ArName = product.ArName,
                    Price = product.Price,
                    ImageUrl = string.IsNullOrEmpty(product.ImageUrl) ? null
                        : product.ImageUrl.StartsWith("http") || product.ImageUrl.StartsWith("/")
                            ? product.ImageUrl
                            : $"/files/{Path.GetFileName(product.ImageUrl)}",
                    Quantity = request.Quantity,
                    QuantityInStock = product.QuantityInStock
                });
            }

            _cartStorage.SetCart(cart);
            return _cartStorage.GetCartCount();
        }
    }
}
