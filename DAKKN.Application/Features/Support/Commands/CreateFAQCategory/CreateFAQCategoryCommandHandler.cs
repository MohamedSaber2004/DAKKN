using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateFAQCategory
{
    public class CreateFAQCategoryCommandHandler : IRequestHandler<CreateFAQCategoryCommand, SupportFAQCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateFAQCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportFAQCategoryDto> Handle(CreateFAQCategoryCommand request, CancellationToken ct)
        {
            var category = SupportFAQCategory.Create(request.Name, request.ArName, request.Icon, request.DisplayOrder);
            var repo = _unitOfWork.GetRepository<SupportFAQCategory>();
            await repo.AddAsync(category);
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
