using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var category = await repo.FindByKeyAsync(request.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            var existing = await repo.GetFirstAsync(c => c.CategoryName == request.CategoryName && c.Id != request.Id, cancellationToken);
            if (existing != null)
                throw new BadRequestException(_localizer[LocalizationKeys.Categories.NameExists.Value]);

            category.CategoryName = request.CategoryName;
            category.ArName = request.ArName;
            category.ImageUrl = request.ImageUrl;
            category.IsActive = request.IsActive;
            category.MarkAsUpdated(_currentUserService.UserId.ToString());

            if (!request.IsActive && !category.IsDeleted)
                category.MarkAsDeleted(_currentUserService.UserId.ToString());
            else if (request.IsActive && category.IsDeleted)
                category.MarkAsRestored();

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ArName = category.ArName,
                ImageUrl = category.ImageUrl,
                IsDeleted = category.IsDeleted
            };
        }
    }
}
