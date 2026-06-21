using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();
            var order = await orderRepo.FindByKeyAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new NotFoundException(nameof(Order), request.OrderId);

            order.MarkAsDeleted(_currentUser.UserId.ToString());
            orderRepo.Update(order);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
