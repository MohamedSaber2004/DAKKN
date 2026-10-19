using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.FindByKeyAsync(request.CategoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException(_localizer[LocalizationKeys.Products.CategoryNotFound.Value]);

            var images = (request.Images ?? new List<string>())
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .ToList();

            var product = new Product
            {
                Name = request.Name,
                ArName = request.ArName,
                Description = request.Description,
                ArDescription = request.ArDescription,
                Price = request.Price,
                // Cover invariant: ImageUrl always mirrors the first gallery image. Derived here
                // rather than trusted from the request so no caller can desync the two.
                ImageUrl = images.FirstOrDefault() ?? request.ImageUrl,
                Images = images,
                FinishOptions = request.FinishOptions,
                SizeOptions = request.SizeOptions,
                CategoryId = request.CategoryId,
                QuantityInStock = request.QuantityInStock,
                DangerQuantity = request.DangerQuantity,
                IsFeatured = request.IsFeatured,
                LastStockUpdateDate = DateTime.UtcNow
            };

            var productRepo = _unitOfWork.GetRepository<Product>();
            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

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
                ImageUrl = product.ImageUrl,
                Images = images,
                FinishOptions = product.FinishOptions,
                SizeOptions = product.SizeOptions,
                CategoryId = product.CategoryId,
                CategoryName = category.CategoryName,
                CategoryArName = category.ArName,
                QuantityInStock = product.QuantityInStock,
                DangerQuantity = product.DangerQuantity,
                StockStatus = product.StockStatus.ToString(),
                IsInStock = product.IsInStock,
                IsFeatured = product.IsFeatured
            };
        }
    }
}
