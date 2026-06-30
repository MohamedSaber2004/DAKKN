using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateFAQCategory
{
    public class UpdateFAQCategoryCommandHandler : IRequestHandler<UpdateFAQCategoryCommand, SupportFAQCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateFAQCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportFAQCategoryDto> Handle(UpdateFAQCategoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportFAQCategory>();
            var category = await repo.GetByIdAsync(request.Id);

            category.Name = request.Name;
            category.ArName = request.ArName;
            category.Icon = request.Icon;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;
            category.MarkAsUpdated(_currentUserService.UserId.ToString());

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new SupportFAQCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ArName = category.ArName,
                Icon = category.Icon,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };
        }
    }
}
