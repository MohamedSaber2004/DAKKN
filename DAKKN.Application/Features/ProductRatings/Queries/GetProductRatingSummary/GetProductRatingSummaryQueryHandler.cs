using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.ProductRatings.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ProductRatings.Queries.GetProductRatingSummary
{
    public class GetProductRatingSummaryQueryHandler : IRequestHandler<GetProductRatingSummaryQuery, ProductRatingSummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetProductRatingSummaryQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<ProductRatingSummaryDto> Handle(GetProductRatingSummaryQuery request, CancellationToken cancellationToken)
        {
            var productRepo = _unitOfWork.GetRepository<Product>();
            var product = await productRepo.GetAllAsync(null)
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

            if (product is null)
                throw new NotFoundException(_localizer[LocalizationKeys.ProductRatings.ProductNotFound.Value]);

            var ratingRepo = _unitOfWork.GetRepository<ProductRating>();
            var query = ratingRepo.GetAllAsync(null).Where(r => r.ProductId == request.ProductId && !r.IsDeleted);

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
                    ProductId = request.ProductId,
                    AverageStars = 0,
                    TotalRatings = 0
                };
            }

            if (_currentUserService.IsAuthenticated)
            {
                var currentUserRating = await query
                    .Where(r => r.UserId == _currentUserService.UserId)
                    .Select(r => (int?)r.Stars)
                    .FirstOrDefaultAsync(cancellationToken);

                summary.CurrentUserStars = currentUserRating;
            }

            return summary;
        }
    }
}
