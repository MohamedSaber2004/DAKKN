using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateFAQ
{
    public record CreateFAQCommand : IRequest<SupportFAQDto>
    {
        public string Question { get; set; } = string.Empty;
        public string ArQuestion { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string ArAnswer { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
