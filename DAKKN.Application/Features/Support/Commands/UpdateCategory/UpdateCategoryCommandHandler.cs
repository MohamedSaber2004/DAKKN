using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, SupportCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportCategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportCategory>();
            var category = await repo.GetByIdAsync(request.Id);

            category.Name = request.Name;
            category.ArName = request.ArName;
            category.Description = request.Description;
            category.Icon = request.Icon;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;
            category.MarkAsUpdated(_currentUserService.UserId.ToString());

            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return new SupportCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ArName = category.ArName,
                Description = category.Description,
                Icon = category.Icon,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };
        }
    }
}
