using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteFAQ
{
    public class DeleteFAQCommandHandler : IRequestHandler<DeleteFAQCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFAQCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteFAQCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportFAQ>();
            var faq = await repo.GetByIdAsync(request.Id);
            repo.Delete(faq);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
