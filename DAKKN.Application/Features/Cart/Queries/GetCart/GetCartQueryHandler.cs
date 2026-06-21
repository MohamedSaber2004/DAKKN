using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Cart.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly IGuestCartStorage _cartStorage;
        private readonly IUnitOfWork _unitOfWork;

        public GetCartQueryHandler(IGuestCartStorage cartStorage, IUnitOfWork unitOfWork)
        {
            _cartStorage = cartStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var items = _cartStorage.GetCart();
            var dto = new CartDto { Items = items };

            var govId = _cartStorage.GetShippingGovernorateId();
            if (govId.HasValue)
            {
                var gov = await _unitOfWork.GetRepository<ShippingGovernorate>()
                    .FindByKeyAsync(govId.Value, cancellationToken);

                if (gov != null && gov.IsActive && !gov.IsDeleted)
                {
                    dto.ShippingGovernorateId = gov.Id;
                    dto.GovernorateName = gov.Name;
                    dto.GovernorateArName = gov.ArName;
                    dto.ShippingPrice = gov.ShippingPrice;
                }
            }

            return dto;
        }
    }
}
