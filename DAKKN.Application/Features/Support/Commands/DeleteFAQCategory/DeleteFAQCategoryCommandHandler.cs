using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteFAQCategory
{
    public class DeleteFAQCategoryCommandHandler : IRequestHandler<DeleteFAQCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFAQCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteFAQCategoryCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportFAQCategory>();
            var category = await repo.GetByIdAsync(request.Id);
            repo.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
