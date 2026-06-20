using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.CreateShippingGovernorate
{
    public class CreateShippingGovernorateCommandHandler : IRequestHandler<CreateShippingGovernorateCommand, ShippingGovernorateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateShippingGovernorateCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<ShippingGovernorateDto> Handle(CreateShippingGovernorateCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();

            var existing = await repo.GetFirstAsync(x => x.Name == request.Name && !x.IsDeleted, cancellationToken);
            if (existing != null)
                throw new BadRequestException(_localizer[LocalizationKeys.ShippingMessages.NameExists.Key]);

            var entity = new ShippingGovernorate
            {
                Name = request.Name,
                ArName = request.ArName,
                ShippingPrice = request.ShippingPrice,
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            await repo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new ShippingGovernorateDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ArName = entity.ArName,
                ShippingPrice = entity.ShippingPrice,
                IsActive = entity.IsActive,
                DisplayOrder = entity.DisplayOrder
            };
        }
    }
}
