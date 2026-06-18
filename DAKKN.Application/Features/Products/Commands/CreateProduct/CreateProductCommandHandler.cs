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

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                FinishOptions = request.FinishOptions,
                SizeOptions = request.SizeOptions,
                CategoryId = request.CategoryId
            };

            var productRepo = _unitOfWork.GetRepository<Product>();
            await productRepo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                ImageUrl = product.ImageUrl,
                FinishOptions = product.FinishOptions,
                SizeOptions = product.SizeOptions,
                CategoryId = product.CategoryId,
                CategoryName = category.CategoryName
            };
        }
    }
}
