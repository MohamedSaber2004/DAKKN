using DAKKN.Application.Common.Exceptions;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.ToggleShippingGovernorateStatus
{
    public class ToggleShippingGovernorateStatusCommandHandler : IRequestHandler<ToggleShippingGovernorateStatusCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleShippingGovernorateStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ToggleShippingGovernorateStatusCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var entity = await repo.FindByKeyAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException(nameof(ShippingGovernorate), request.Id);

            entity.IsActive = !entity.IsActive;
            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
