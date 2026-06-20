using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartShipping
{
    public class UpdateCartShippingCommandHandler : IRequestHandler<UpdateCartShippingCommand, CartDto>
    {
        private readonly IGuestCartStorage _cartStorage;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCartShippingCommandHandler(IGuestCartStorage cartStorage, IUnitOfWork unitOfWork)
        {
            _cartStorage = cartStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(UpdateCartShippingCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var gov = await repo.FindByKeyAsync(request.ShippingGovernorateId, cancellationToken);
            if (gov == null || !gov.IsActive || gov.IsDeleted)
                throw new NotFoundException(nameof(ShippingGovernorate), request.ShippingGovernorateId);

            var items = _cartStorage.GetCart();
            return new CartDto
            {
                Items = items,
                ShippingGovernorateId = gov.Id,
                GovernorateName = gov.Name,
                GovernorateArName = gov.ArName,
                ShippingPrice = gov.ShippingPrice
            };
        }
    }
}
