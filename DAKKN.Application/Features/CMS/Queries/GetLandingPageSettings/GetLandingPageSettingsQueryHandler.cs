using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CMS.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings
{
    public class GetLandingPageSettingsQueryHandler : IRequestHandler<GetLandingPageSettingsQuery, LandingPageSettingsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetLandingPageSettingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LandingPageSettingsDto> Handle(GetLandingPageSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await _context.LandingPageSettings
                .ToListAsync(cancellationToken);

            var dto = new LandingPageSettingsDto();

            foreach (var setting in settings)
            {
                switch (setting.Key)
                {
                    case "cms_section_order": dto.SectionOrder = setting.Value; break;
                    case "cms_hero":          dto.Hero = setting.Value; break;
                    case "cms_about":         dto.About = setting.Value; break;
                    case "cms_categories":    dto.Categories = setting.Value; break;
                    case "cms_products":      dto.Products = setting.Value; break;
                    case "cms_testimonials":  dto.Testimonials = setting.Value; break;
                    case "cms_contact":       dto.Contact = setting.Value; break;
                }
            }

            return dto;
        }
    }
}
