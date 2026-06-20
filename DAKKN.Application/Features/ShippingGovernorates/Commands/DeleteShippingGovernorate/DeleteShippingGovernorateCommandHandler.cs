using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.DeleteShippingGovernorate
{
    public class DeleteShippingGovernorateCommandHandler : IRequestHandler<DeleteShippingGovernorateCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteShippingGovernorateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteShippingGovernorateCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var entity = await repo.FindByKeyAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException(nameof(ShippingGovernorate), request.Id);

            entity.MarkAsDeleted(_currentUser.UserId.ToString() ?? "System");
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
