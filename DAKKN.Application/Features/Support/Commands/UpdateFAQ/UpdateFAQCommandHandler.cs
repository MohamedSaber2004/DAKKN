using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateFAQ
{
    public class UpdateFAQCommandHandler : IRequestHandler<UpdateFAQCommand, SupportFAQDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateFAQCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportFAQDto> Handle(UpdateFAQCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportFAQ>();
            var faq = await repo.GetByIdAsync(request.Id);

            faq.Question = request.Question;
            faq.ArQuestion = request.ArQuestion;
            faq.Answer = request.Answer;
            faq.ArAnswer = request.ArAnswer;
            faq.CategoryId = request.CategoryId;
            faq.DisplayOrder = request.DisplayOrder;
            faq.IsPublished = request.IsPublished;
            faq.MarkAsUpdated(_currentUserService.UserId.ToString());

            repo.Update(faq);
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
