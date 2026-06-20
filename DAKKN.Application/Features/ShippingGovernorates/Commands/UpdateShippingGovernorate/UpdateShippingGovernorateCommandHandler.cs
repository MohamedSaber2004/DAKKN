using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.UpdateShippingGovernorate
{
    public class UpdateShippingGovernorateCommandHandler : IRequestHandler<UpdateShippingGovernorateCommand, ShippingGovernorateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateShippingGovernorateCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<ShippingGovernorateDto> Handle(UpdateShippingGovernorateCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var entity = await repo.FindByKeyAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new NotFoundException(nameof(ShippingGovernorate), request.Id);

            var duplicate = await repo.GetFirstAsync(x => x.Name == request.Name && x.Id != request.Id && !x.IsDeleted, cancellationToken);
            if (duplicate != null)
                throw new BadRequestException(_localizer[LocalizationKeys.ShippingMessages.NameExists.Key]);

            entity.Name = request.Name;
            entity.ArName = request.ArName;
            entity.ShippingPrice = request.ShippingPrice;
            entity.DisplayOrder = request.DisplayOrder;
            entity.IsActive = request.IsActive;

            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new ShippingGovernorateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ArName = entity.ArName,
                ShippingPrice = entity.ShippingPrice,
                IsActive = entity.IsActive && !entity.IsDeleted,
                DisplayOrder = entity.DisplayOrder
            };
        }
    }
}
