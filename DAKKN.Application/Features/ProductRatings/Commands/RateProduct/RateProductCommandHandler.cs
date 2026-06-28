using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.ProductRatings.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ProductRatings.Commands.RateProduct
{
    public class RateProductCommandHandler : IRequestHandler<RateProductCommand, ProductRatingSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public RateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<ProductRatingSummaryDto> Handle(RateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var productRepo = _unitOfWork.GetRepository<Product>();
            var product = await productRepo.GetAllAsync(null)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

            if (product is null)
                throw new NotFoundException(_localizer[LocalizationKeys.ProductRatings.ProductNotFound.Value]);

            var ratingRepo = _unitOfWork.GetRepository<ProductRating>();
            var existing = await ratingRepo.GetAllAsync(null)
                .FirstOrDefaultAsync(r => r.ProductId == request.ProductId && r.UserId == userId, cancellationToken);

            if (existing is not null)
            {
                existing.Stars = request.Stars;
                ratingRepo.Update(existing);
            }
            else
            {
                var rating = new ProductRating
                {
                    ProductId = request.ProductId,
                    UserId = userId,
                    Stars = request.Stars
                };
                await ratingRepo.AddAsync(rating);
            }

            await _unitOfWork.SaveChangesAsync();

            var summary = await GetSummaryAsync(request.ProductId, userId, cancellationToken);

            product.AverageRating = summary.AverageStars;
            product.ReviewCount = summary.TotalRatings;
            await _unitOfWork.SaveChangesAsync();

            return summary;
        }

        private async Task<ProductRatingSummaryDto> GetSummaryAsync(Guid productId, Guid userId, CancellationToken cancellationToken)
        {
            var ratingRepo = _unitOfWork.GetRepository<ProductRating>();
            var query = ratingRepo.GetAllAsync(null).Where(r => r.ProductId == productId && !r.IsDeleted);

            var summary = await query.GroupBy(r => r.ProductId)
                .Select(g => new ProductRatingSummaryDto
                {
                    ProductId = g.Key,
                    AverageStars = Math.Round(g.Average(r => r.Stars), 1),
                    TotalRatings = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (summary is null)
            {
                summary = new ProductRatingSummaryDto
                {
                    ProductId = productId,
                    AverageStars = 0,
                    TotalRatings = 0
                };
            }

            var currentUserRating = await query
                .Where(r => r.UserId == userId)
                .Select(r => (int?)r.Stars)
                .FirstOrDefaultAsync(cancellationToken);

            summary.CurrentUserStars = currentUserRating;

            return summary;
        }
    }
}
