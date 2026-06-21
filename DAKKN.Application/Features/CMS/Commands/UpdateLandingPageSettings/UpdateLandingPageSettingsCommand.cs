using DAKKN.Application.Features.CMS.DTOs;
using MediatR;

namespace DAKKN.Application.Features.CMS.Commands.UpdateLandingPageSettings
{
    public record UpdateLandingPageSettingsCommand : IRequest<LandingPageSettingsDto>
    {
        public string? SectionOrder { get; init; }
        public string? Hero { get; init; }
        public string? About { get; init; }
        public string? Categories { get; init; }
        public string? Products { get; init; }
        public string? Testimonials { get; init; }
        public string? Contact { get; init; }
    }
}
