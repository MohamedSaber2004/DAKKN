using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateFAQCategory
{
    public record CreateFAQCategoryCommand : IRequest<SupportFAQCategoryDto>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
    }
}
