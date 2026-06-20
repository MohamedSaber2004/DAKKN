using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuestCartStorage _cartStorage;

        public AddToCartCommandHandler(IUnitOfWork unitOfWork, IGuestCartStorage cartStorage)
        {
            _unitOfWork = unitOfWork;
            _cartStorage = cartStorage;
        }

        public async Task<int> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var product = await repo.FindByKeyAsync(request.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.ProductId);

            if (!product.IsActive || product.IsDeleted)
                throw new BadRequestException("Product is not available");

            var cart = _cartStorage.GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);

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
                            : $"/files/{product.ImageUrl}",
                    Quantity = request.Quantity
                });
            }

            _cartStorage.SetCart(cart);
            return _cartStorage.GetCartCount();
        }
    }
}
