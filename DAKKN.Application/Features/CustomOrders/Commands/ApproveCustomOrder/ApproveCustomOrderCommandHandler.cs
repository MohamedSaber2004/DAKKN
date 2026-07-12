using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.CustomOrders.Commands.ApproveCustomOrder
{
    public class ApproveCustomOrderCommandHandler : IRequestHandler<ApproveCustomOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IStringLocalizer<Messages> _localizer;

        public ApproveCustomOrderCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public async Task<bool> Handle(ApproveCustomOrderCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<CustomOrder>();
            var order = await repo.FindByKeyAsync(request.Id, cancellationToken);

            if (order == null || order.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.CustomOrderSubmission.NotFound.Value]);

            order.Approve();
            order.MarkAsUpdated(_currentUser.UserId.ToString());
            repo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
