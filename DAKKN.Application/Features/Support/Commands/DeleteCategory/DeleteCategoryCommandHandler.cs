using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportCategory>();
            var category = await repo.GetByIdAsync(request.Id);

            if (category == null)
                throw new NotFoundException(nameof(SupportCategory), request.Id);

            category.MarkAsDeleted(_currentUserService.UserId.ToString());
            repo.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
