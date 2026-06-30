using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetFAQs
{
    public class GetFAQsQueryHandler : IRequestHandler<GetFAQsQuery, List<SupportFAQDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFAQsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupportFAQDto>> Handle(GetFAQsQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportFAQ>()
                .GetAllAsync(_ => true)
                .Include(f => f.Category)
                .AsQueryable();

            if (request.OnlyPublished)
                query = query.Where(f => f.IsPublished);

            if (request.CategoryId.HasValue)
                query = query.Where(f => f.CategoryId == request.CategoryId.Value);

            return await query.OrderBy(f => f.DisplayOrder).Select(f => new SupportFAQDto
            {
                Id = f.Id,
                Question = f.Question,
                ArQuestion = f.ArQuestion,
                Answer = f.Answer,
                ArAnswer = f.ArAnswer,
                CategoryName = f.Category.Name,
                CategoryArName = f.Category.ArName,
                CategoryId = f.CategoryId,
                DisplayOrder = f.DisplayOrder,
                IsPublished = f.IsPublished
            }).ToListAsync(ct);
        }
    }
}
