using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.GetRepository<Product>();
            var product = await productRepo.FindByKeyAsync(request.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.Id);

            var categoryRepo = _unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.FindByKeyAsync(request.CategoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException(_localizer[LocalizationKeys.Products.CategoryNotFound.Value]);

            product.Name = request.Name;
            product.ArName = request.ArName;
            product.Description = request.Description;
            product.ArDescription = request.ArDescription;
            product.Price = request.Price;
            product.ImageUrl = request.ImageUrl;
            product.FinishOptions = request.FinishOptions;
            product.SizeOptions = request.SizeOptions;
            product.CategoryId = request.CategoryId;
            product.QuantityInStock = request.QuantityInStock;
            product.DangerQuantity = request.DangerQuantity;
            product.LastStockUpdateDate = DateTime.UtcNow;

            productRepo.Update(product);
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
                FinishOptions = product.FinishOptions,
                SizeOptions = product.SizeOptions,
                CategoryId = product.CategoryId,
                CategoryName = category.CategoryName,
                CategoryArName = category.ArName,
                QuantityInStock = product.QuantityInStock,
                DangerQuantity = product.DangerQuantity,
                StockStatus = product.StockStatus.ToString(),
                IsInStock = product.IsInStock
            };
        }
    }
}
