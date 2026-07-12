using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Features.CustomOrders.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrderById
{
    public class GetCustomOrderByIdQueryHandler : IRequestHandler<GetCustomOrderByIdQuery, CustomOrderDetailDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public GetCustomOrderByIdQueryHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<CustomOrderDetailDto?> Handle(GetCustomOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<CustomOrder>();
            var order = await repo.GetAllAsync(null).AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == request.Id && !o.IsDeleted, cancellationToken);

            if (order == null)
                throw new NotFoundException(_localizer[LocalizationKeys.CustomOrderSubmission.NotFound.Value]);

            var imageUrl = order.ImageUrl;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = string.Join(",", imageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => f.StartsWith("http") || f.StartsWith("/") ? f : $"/files/{f}"));
            }

            return new CustomOrderDetailDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                ImageUrl = imageUrl,
                Shape = order.Shape,
                Size = order.Size,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }
}
