using DAKKN.Application.Common.Exceptions;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Products.Commands.ToggleProductFeatured
{
    public class ToggleProductFeaturedCommandHandler : IRequestHandler<ToggleProductFeaturedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleProductFeaturedCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ToggleProductFeaturedCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var product = await repo.FindByKeyAsync(request.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.ProductId);

            product.ToggleFeatured();
            repo.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return product.IsFeatured;
        }
    }
}
