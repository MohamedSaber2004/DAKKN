using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RemoveFavoriteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<UserFavorite>();
            var favorite = await repo.GetAllAsync(f => f.UserId == _currentUserService.UserId && f.ProductId == request.ProductId && !f.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (favorite == null)
                throw new NotFoundException(nameof(UserFavorite), request.ProductId);

            repo.Delete(favorite);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
