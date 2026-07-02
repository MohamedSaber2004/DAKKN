using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Inventory.Commands.ApplyGlobalDangerQuantity
{
    public class ApplyGlobalDangerQuantityCommandHandler : IRequestHandler<ApplyGlobalDangerQuantityCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplyGlobalDangerQuantityCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(ApplyGlobalDangerQuantityCommand request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.GetRepository<SystemSetting>()
                .GetFirstAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            var globalValue = setting != null && int.TryParse(setting.Value, out var val) ? val : 10;

            var products = await _unitOfWork.GetRepository<Product>()
                .GetBy(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.DangerQuantity = globalValue;
            }

            await _unitOfWork.SaveChangesAsync();

            return products.Count;
        }
    }
}
