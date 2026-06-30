using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Favorites.Commands.ToggleFavorite
{
    public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ToggleFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<UserFavorite>();
            var existing = await repo.GetAllAsync(f => f.UserId == _currentUserService.UserId && f.ProductId == request.ProductId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != null)
            {
                if (!existing.IsDeleted)
                {
                    repo.Delete(existing);
                    await _unitOfWork.SaveChangesAsync();
                    return false;
                }
                else
                {
                    existing.MarkAsRestored();
                    repo.Update(existing);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            var favorite = new UserFavorite
            {
                UserId = _currentUserService.UserId,
                ProductId = request.ProductId
            };
            await repo.AddAsync(favorite);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
