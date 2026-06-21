using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Inventory.Commands.ApplyGlobalDangerQuantity
{
    public class ApplyGlobalDangerQuantityCommandHandler : IRequestHandler<ApplyGlobalDangerQuantityCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public ApplyGlobalDangerQuantityCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ApplyGlobalDangerQuantityCommand request, CancellationToken cancellationToken)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            var globalValue = setting != null && int.TryParse(setting.Value, out var val) ? val : 10;

            var products = await _context.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                product.DangerQuantity = globalValue;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return products.Count;
        }
    }
}
