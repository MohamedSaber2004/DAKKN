using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateFAQCategory
{
    public record UpdateFAQCategoryCommand : IRequest<SupportFAQCategoryDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
