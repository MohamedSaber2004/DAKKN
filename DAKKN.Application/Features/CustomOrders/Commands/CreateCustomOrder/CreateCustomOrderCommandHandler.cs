using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CustomOrders.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.CustomOrders.Commands.CreateCustomOrder
{
    public class CreateCustomOrderCommandHandler : IRequestHandler<CreateCustomOrderCommand, CustomOrderDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateCustomOrderCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public async Task<CustomOrderDetailDto> Handle(CreateCustomOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null;

            var order = new CustomOrder(
                request.CustomerName,
                request.CustomerPhone,
                request.ShippingAddress,
                request.Notes,
                request.ImageUrl,
                request.Shape,
                request.Size,
                request.Quantity,
                request.TotalAmount,
                userId);

            order.MarkAsCreated(_currentUser.IsAuthenticated ? _currentUser.UserId.ToString() : "Guest");

            var repo = _unitOfWork.GetRepository<CustomOrder>();
            await repo.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return new CustomOrderDetailDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                ImageUrl = order.ImageUrl,
                Shape = order.Shape,
                Size = order.Size,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
