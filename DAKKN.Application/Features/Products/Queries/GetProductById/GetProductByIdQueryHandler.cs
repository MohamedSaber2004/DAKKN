using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var product = await repo.FindByKeyAsync(request.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.Id);

            var category = await _unitOfWork.GetRepository<Category>().FindByKeyAsync(product.CategoryId, cancellationToken);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ArName = product.ArName,
                Description = product.Description,
                ArDescription = product.ArDescription,
                Price = product.Price,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                ImageUrl = string.IsNullOrEmpty(product.ImageUrl) || product.ImageUrl.StartsWith("http") || product.ImageUrl.StartsWith("/")
                    ? product.ImageUrl
                    : $"/files/{product.ImageUrl}",
                FinishOptions = product.FinishOptions,
                SizeOptions = product.SizeOptions,
                CategoryId = product.CategoryId,
                CategoryName = category?.CategoryName ?? string.Empty,
                CategoryArName = category?.ArName ?? string.Empty,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsActive = product.IsActive,
                IsDeleted = product.IsDeleted
            };
        }
    }
}
