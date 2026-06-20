using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity
{
    public class UpdateCartQuantityCommandHandler : IRequestHandler<UpdateCartQuantityCommand, int>
    {
        private readonly IGuestCartStorage _cartStorage;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCartQuantityCommandHandler(IGuestCartStorage cartStorage, IUnitOfWork unitOfWork)
        {
            _cartStorage = cartStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateCartQuantityCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var product = await repo.FindByKeyAsync(request.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.ProductId);

            if (request.Quantity > product.QuantityInStock)
                throw new BadRequestException($"Only {product.QuantityInStock} items available.");

            var cart = _cartStorage.GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (existing != null)
            {
                existing.Quantity = Math.Max(1, request.Quantity);
            }
            _cartStorage.SetCart(cart);
            return _cartStorage.GetCartCount();
        }
    }
}
