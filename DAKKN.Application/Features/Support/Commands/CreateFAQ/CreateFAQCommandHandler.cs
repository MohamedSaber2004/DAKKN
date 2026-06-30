using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateFAQ
{
    public class CreateFAQCommandHandler : IRequestHandler<CreateFAQCommand, SupportFAQDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateFAQCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportFAQDto> Handle(CreateFAQCommand request, CancellationToken ct)
        {
            var faq = SupportFAQ.Create(
                request.Question, request.ArQuestion,
                request.Answer, request.ArAnswer,
                request.CategoryId, request.DisplayOrder);

            faq.IsPublished = request.IsPublished;
            var repo = _unitOfWork.GetRepository<SupportFAQ>();
            await repo.AddAsync(faq);
            await _unitOfWork.SaveChangesAsync();

            return new SupportFAQDto
            {
                Id = faq.Id,
                Question = faq.Question,
                ArQuestion = faq.ArQuestion,
                Answer = faq.Answer,
                ArAnswer = faq.ArAnswer,
                CategoryId = faq.CategoryId,
                DisplayOrder = faq.DisplayOrder,
                IsPublished = faq.IsPublished
            };
        }
    }
}
