using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateOrderStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var historyRepo = _unitOfWork.GetRepository<OrderStatusHistory>();

            var order = await orderRepo.FindByKeyAsync(request.OrderId, cancellationToken);
            if (order == null || order.IsDeleted)
                throw new NotFoundException(nameof(Order), request.OrderId);

            if (!order.CanTransitionTo(request.NewStatus))
                throw new BadRequestException(
                    string.Format(_localizer[LocalizationKeys.OrderMessages.InvalidStatusTransition.Value],
                        order.Status, request.NewStatus));

            var oldStatus = order.Status;
            var changedBy = _currentUser.IsAuthenticated ? _currentUser.UserId.ToString() : "System";

            order.UpdateStatus(request.NewStatus, changedBy);
            orderRepo.Update(order);

            var history = new OrderStatusHistory(
                order.Id, oldStatus, request.NewStatus, changedBy, request.Notes);
            await historyRepo.AddAsync(history);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
