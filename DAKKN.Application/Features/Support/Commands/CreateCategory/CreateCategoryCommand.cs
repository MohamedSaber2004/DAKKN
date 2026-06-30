using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CreateCategory
{
    public record CreateCategoryCommand : IRequest<SupportCategoryDto>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
    }
}
