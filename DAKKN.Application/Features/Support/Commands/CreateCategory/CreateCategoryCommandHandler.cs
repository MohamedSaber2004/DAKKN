using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, SupportCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportCategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var category = SupportCategory.Create(
                request.Name, request.ArName, request.Description, request.Icon, request.DisplayOrder);

            var repo = _unitOfWork.GetRepository<SupportCategory>();
            await repo.AddAsync(category);
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
